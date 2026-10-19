using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[Serializable]
public class EnemyPoolEntry
{
    [SerializeField] private EnemyHealth enemyPrefab;
    [SerializeField] private int defaultCapacity = 5;
    [SerializeField] private int maxSize = 20;

    public EnemyHealth EnemyPrefab => enemyPrefab;
    public int DefaultCapacity => defaultCapacity;
    public int MaxSize => maxSize;
}

public class EnemyPool : MonoBehaviour
{
    [Header("Enemy Pool Entries")]
    [SerializeField]
    private List<EnemyPoolEntry> enemyPoolEntries = new List<EnemyPoolEntry>();

    private readonly Dictionary<EnemyHealth, ObjectPool<EnemyHealth>> poolsByPrefab = new Dictionary<EnemyHealth, ObjectPool<EnemyHealth>>();

    private readonly Dictionary<EnemyHealth, ObjectPool<EnemyHealth>> ownerPoolByEnemy = new Dictionary<EnemyHealth, ObjectPool<EnemyHealth>>();

    private readonly HashSet<EnemyHealth> rentedEnemies = new HashSet<EnemyHealth>();

    private void Awake()
    {
        ValidateEnemyPoolEntries();

        foreach (EnemyPoolEntry poolEntry in enemyPoolEntries)
        {
            if (poolEntry == null)
            {
                continue;
            }

            CreatePoolIfNeeded(poolEntry.EnemyPrefab, poolEntry.DefaultCapacity, poolEntry.MaxSize);
        }
    }

    private void ValidateEnemyPoolEntries()
    {
        HashSet<EnemyHealth> configuredPrefabs =
            new HashSet<EnemyHealth>();

        for (int entryIndex = 0;
             entryIndex < enemyPoolEntries.Count;
             entryIndex++)
        {
            EnemyPoolEntry poolEntry = enemyPoolEntries[entryIndex];

            if (poolEntry == null)
            {
                Debug.LogError(
                    $"EnemyPool Entry {entryIndex} is missing.",
                    this
                );

                continue;
            }

            EnemyHealth entryPrefab = poolEntry.EnemyPrefab;

            if (entryPrefab == null)
            {
                Debug.LogError(
                    $"EnemyPool Entry {entryIndex} has no Enemy Prefab assigned.",
                    this
                );

                continue;
            }

            if (!configuredPrefabs.Add(entryPrefab))
            {
                Debug.LogError(
                    $"EnemyPool has duplicate entries for prefab " +
                    $"'{entryPrefab.name}'. Each Enemy prefab needs only one Pool Entry.",
                    this
                );
            }

            if (poolEntry.DefaultCapacity < 0)
            {
                Debug.LogError(
                    $"EnemyPool Entry '{entryPrefab.name}' has an invalid " +
                    $"Default Capacity. It must be 0 or greater.",
                    this
                );
            }

            if (poolEntry.MaxSize <= 0)
            {
                Debug.LogError(
                    $"EnemyPool Entry '{entryPrefab.name}' has an invalid " +
                    $"Max Size. It must be greater than 0.",
                    this
                );
            }

            if (poolEntry.DefaultCapacity > poolEntry.MaxSize)
            {
                Debug.LogWarning(
                    $"EnemyPool Entry '{entryPrefab.name}' has Default Capacity " +
                    $"greater than Max Size.",
                    this
                );
            }
        }
    }

    private void CreatePoolIfNeeded(EnemyHealth prefab, int defaultCapacity, int maxSize)
    {
        if (prefab == null)
        {
            return;
        }

        if (defaultCapacity < 0 ||
            maxSize <= 0 ||
            defaultCapacity > maxSize)
        {
            return;
        }

        if (poolsByPrefab.ContainsKey(prefab))
        {
            return;
        }

        ObjectPool<EnemyHealth> newPool = new ObjectPool<EnemyHealth>(() =>
            CreateEnemy(prefab),
            OnGetEnemy,
            OnReleaseEnemy,
            OnDestroyEnemy,
            true,
            defaultCapacity,
            maxSize
        );

        poolsByPrefab.Add(prefab, newPool);
    }

    private EnemyHealth CreateEnemy(EnemyHealth prefab)
    {
        EnemyHealth createdEnemy = Instantiate(prefab, transform);

        createdEnemy.gameObject.SetActive(false);

        return createdEnemy;
    }

    private void OnGetEnemy(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null)
        {
            return;
        }

        rentedEnemies.Add(enemyHealth);

        enemyHealth.ResetForReuse();

        EnemyAI enemyAI = enemyHealth.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.ResetForReuse();
        }
    }

    private void OnReleaseEnemy(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null)
        {
            return;
        }

        enemyHealth.gameObject.SetActive(false);
        enemyHealth.transform.SetParent(transform);
    }

    private void OnDestroyEnemy(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null)
        {
            return;
        }

        ownerPoolByEnemy.Remove(enemyHealth);
        rentedEnemies.Remove(enemyHealth);

        Destroy(enemyHealth.gameObject);
    }

    public EnemyHealth GetEnemy(EnemyHealth requestedEnemyPrefab, Vector3 spawnPosition, Transform enemyContainer)
    {
        if (requestedEnemyPrefab == null)
        {
            Debug.LogError(
                "EnemyPool cannot spawn because the requested Enemy prefab is missing.",
                this
            );

            return null;
        }

        if (!poolsByPrefab.TryGetValue(requestedEnemyPrefab, out ObjectPool<EnemyHealth> requestedPool))
        {
            Debug.LogError(
                $"EnemyPool has no Pool Entry for prefab " +
                $"'{requestedEnemyPrefab.name}'.",
                this
            );

            return null;
        }

        EnemyHealth enemyHealth = requestedPool.Get();

        // Ghi nhớ pool gốc để khi Enemy chết, nó quay đúng pool con.
        ownerPoolByEnemy[enemyHealth] = requestedPool;

        enemyHealth.transform.SetParent(enemyContainer);
        enemyHealth.transform.position = spawnPosition;
        enemyHealth.transform.rotation = Quaternion.identity;

        return enemyHealth;
    }

    public void ReleaseEnemy(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null)
        {
            return;
        }

        if (!rentedEnemies.Remove(enemyHealth))
        {
            return;
        }

        if (!ownerPoolByEnemy.TryGetValue(enemyHealth, out ObjectPool<EnemyHealth> ownerPool))
        {
            Debug.LogError(
                $"EnemyPool does not know which pool owns '{enemyHealth.name}'.",
                this
            );

            return;
        }

        ownerPool.Release(enemyHealth);
    }

    public void ReleaseAllRentedEnemies()
    {
        List<EnemyHealth> enemiesToRelease = new List<EnemyHealth>(rentedEnemies);

        foreach (EnemyHealth enemyHealth in enemiesToRelease)
        {
            ReleaseEnemy(enemyHealth);
        }
    }
}