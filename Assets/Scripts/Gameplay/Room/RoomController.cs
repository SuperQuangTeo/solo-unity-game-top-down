using System;
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

    private EnemyHealth[] roomEnemies;

    public event Action OnRoomCleared;

    public bool IsRoomActive => isRoomActive;
    public bool IsRoomCleared => isRoomCleared;
    public int AliveEnemyCount => aliveEnemyCount;

    private void Awake()
    {
        isRoomActive = false;
        isRoomCleared = false;
        FindRoomEnemies();
        aliveEnemyCount = roomEnemies.Length;
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


    private void HandleEnemyDeath()
    {
        aliveEnemyCount--;
        aliveEnemyCount = Mathf.Max(aliveEnemyCount, 0);
        CheckRoomCleared();
    }

    public void StartCombat()
    {
        if (isRoomCleared)
        {
            return;
        }
        if (isRoomActive)
        {
            return;
        }
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
}