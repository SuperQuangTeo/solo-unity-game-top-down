using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room State")]
    [SerializeField] private bool isRoomActive;
    [SerializeField] private bool isRoomCleared;

    [Header("Enemy References")]
    [SerializeField] private Transform enemyContainer;

    [Header("Enemy State")]
    [SerializeField] private int aliveEnemyCount;

    [Header("Door References")]
    [SerializeField] private RoomDoor[] roomDoors;

    [Header("Runtime Gameplay References")]
    private Transform playerTransform;
    private CameraController cameraController;


    private EnemyPool enemyPool;

    private EnemyHealth[] roomEnemies;

    private RoomContext roomContext;

    public event Action OnRoomCleared;

    private bool hasSpawnedRuntimeEnemies;

    private bool usesEnemyPoolForRuntimeEnemies;

    public bool IsRoomActive => isRoomActive;
    public bool IsRoomCleared => isRoomCleared;
    public int AliveEnemyCount => aliveEnemyCount;

    private void Awake()
    {
        roomContext = GetComponent<RoomContext>();

        isRoomActive = false;
        isRoomCleared = false;
        FindRoomEnemies();
        aliveEnemyCount = roomEnemies.Length;
        hasSpawnedRuntimeEnemies = roomEnemies.Length > 0;
    }

    private void OnEnable()
    {
        SubscribeToEnemyEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEnemyEvents();
    }

    private void FindRoomEnemies()
    {
        if (enemyContainer == null)
        {
            roomEnemies = new EnemyHealth[0];
            return;
        }
        roomEnemies = enemyContainer.GetComponentsInChildren<EnemyHealth>(true);
    }

    public void SetRuntimeReferences(Transform newPlayerTransform, CameraController newCameraController)
    {
        playerTransform = newPlayerTransform;
        cameraController = newCameraController;
    }

    public void SetEnemyPool(EnemyPool newEnemyPool)
    {
        enemyPool = newEnemyPool;
    }

    private void SubscribeToEnemyEvents()
    {
        foreach (EnemyHealth enemyHealth in roomEnemies)
        {
            if (enemyHealth == null)
            {
                continue;
            }
            enemyHealth.OnDeath += HandleEnemyDeath;
        }
    }

    private void UnsubscribeFromEnemyEvents()
    {
        foreach (EnemyHealth enemyHealth in roomEnemies)
        {
            if (enemyHealth == null)
            {
                continue;
            }
            enemyHealth.OnDeath -= HandleEnemyDeath;
        }
    }

    private void HandleEnemyDeath(EnemyHealth deadEnemyHealth)
    {
        if (deadEnemyHealth == null)
        {
            return;
        }

        deadEnemyHealth.OnDeath -= HandleEnemyDeath;

        aliveEnemyCount--;

        aliveEnemyCount = Mathf.Max(aliveEnemyCount, 0);

        CheckRoomCleared();

        if (usesEnemyPoolForRuntimeEnemies && enemyPool != null)
        {
            StartCoroutine(
        ReleaseEnemyNextFrame(deadEnemyHealth)
    );
        }
    }

    private IEnumerator ReleaseEnemyNextFrame(
    EnemyHealth deadEnemyHealth)
    {
        yield return null;

        if (deadEnemyHealth == null)
        {
            yield break;
        }

        if (enemyPool == null)
        {
            yield break;
        }

        enemyPool.ReleaseEnemy(deadEnemyHealth);
    }

    public void StartCombat()
    {
        if (!IsCombatRoom())
        {
            return;
        }
        if (isRoomCleared)
        {
            return;
        }
        if (isRoomActive)
        {
            return;
        }

        PrepareRuntimeEnemies();

        isRoomActive = true;

        LockDoors();
        ActivateEnemies();
        CheckRoomCleared();
    }

    private void CheckRoomCleared()
    {
        if (!isRoomActive)
        {
            return;
        }
        if (isRoomCleared)
        {
            return;
        }
        if (aliveEnemyCount > 0)
        {
            return;
        }
        isRoomCleared = true;
        isRoomActive = false;

        UnlockDoors();

        OnRoomCleared?.Invoke();
    }

    private void LockDoors()
    {
        foreach (RoomDoor roomDoor in roomDoors)
        {
            if (roomDoor == null)
            {
                continue;
            }
            roomDoor.LockDoor();
        }
    }
    private void UnlockDoors()
    {
        foreach (RoomDoor roomDoor in roomDoors)
        {
            if (roomDoor == null)
            {
                continue;
            }
            roomDoor.UnlockDoor();
        }
    }
    private void ActivateEnemies()
    {
        foreach (EnemyHealth enemyHealth in roomEnemies)
        {
            if (enemyHealth == null)
            {
                continue;
            }
            enemyHealth.gameObject.SetActive(true);
        }
    }
    public void SetRoomDoors(RoomDoor[] newRoomDoors)
    {
        roomDoors = newRoomDoors;
    }

    private bool IsCombatRoom()
    {
        if (roomContext == null)
        {
            return true;
        }
        if (roomContext.RoomType == RoomType.Combat)
        {
            return true;
        }
        if (roomContext.RoomType == RoomType.Boss)
        {
            return true;
        }
        return false;
    }

    private void PrepareRuntimeEnemies()
    {
        if (hasSpawnedRuntimeEnemies)
        {
            return;
        }

        if (roomContext == null)
        {
            return;
        }

        EnemySpawnPoint[] enemySpawnPoints = roomContext.EnemySpawnPoints;

        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0)
        {
            Debug.LogWarning(
                $"Room '{gameObject.name}' has no EnemySpawnPoint.",
                this
            );

            return;
        }

        if (enemyPool == null)
        {
            Debug.LogError(
                $"Room '{gameObject.name}' has no EnemyPool runtime reference.",
                this
            );

            return;
        }

        if (enemyContainer == null)
        {
            Debug.LogError(
                $"Room '{gameObject.name}' is not attached Enemy Container.",
                this
            );
            return;
        }

        List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();

        foreach (EnemySpawnPoint enemySpawnPoint in enemySpawnPoints)
        {
            if (enemySpawnPoint == null)
            {
                continue;
            }

            EnemyHealth spawnedEnemy = enemyPool.GetEnemy(enemySpawnPoint.transform.position, enemyContainer);

            ConfigureSpawnedEnemy(spawnedEnemy);

            spawnedEnemies.Add(spawnedEnemy);
        }
        UnsubscribeFromEnemyEvents();

        roomEnemies = spawnedEnemies.ToArray();

        aliveEnemyCount = roomEnemies.Length;

        SubscribeToEnemyEvents();

        hasSpawnedRuntimeEnemies = true;
        usesEnemyPoolForRuntimeEnemies = true;
    }

    private void ConfigureSpawnedEnemy(EnemyHealth spawnedEnemy)
    {
        if (spawnedEnemy == null)
        {
            return;
        }

        EnemyAI enemyAI = spawnedEnemy.GetComponent<EnemyAI>();

        if (enemyAI != null)
        {
            enemyAI.SetPlayerTransform(playerTransform);
        }

        EnemyDeathCameraShake enemyDeathCameraShake = spawnedEnemy.GetComponent<EnemyDeathCameraShake>();

        if (enemyDeathCameraShake != null)
        {
            enemyDeathCameraShake.SetCameraController(cameraController);
        }
    }
}