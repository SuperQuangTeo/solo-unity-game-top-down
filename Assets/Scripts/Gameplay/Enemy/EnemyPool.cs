using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private EnemyHealth enemyPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<EnemyHealth> enemyPool;


    private void Awake()
    {
        enemyPool = new ObjectPool<EnemyHealth>(
            CreateEnemy,
            OnGetEnemy,
            OnReleaseEnemy,
            OnDestroyEnemy,
            true,
            defaultCapacity,
            maxSize
        );
    }


    private EnemyHealth CreateEnemy()
    {
        EnemyHealth createdEnemy = Instantiate(enemyPrefab, transform);

        createdEnemy.gameObject.SetActive(false);

        return createdEnemy;
    }


    private void OnGetEnemy(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null)
        {
            return;
        }

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

        Destroy(enemyHealth.gameObject);
    }


    public EnemyHealth GetEnemy(Vector3 spawnPosition, Transform enemyContainer)
    {
        EnemyHealth enemyHealth = enemyPool.Get();

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

        enemyPool.Release(enemyHealth);
    }
}