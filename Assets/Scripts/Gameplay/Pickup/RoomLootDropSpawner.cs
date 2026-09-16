using UnityEngine;

public class RoomLootDropSpawner : MonoBehaviour
{
    [Header("Room References")]
    [SerializeField] private RoomContext roomContext;
    [SerializeField] private RoomLootRoller roomLootRoller;

    [Header("Pool")]
    [SerializeField] private DropPool dropPool;

    [SerializeField] private RoomMarkerTilemap roomMarkerTilemap;

    private void OnEnable()
    {
        if (roomLootRoller == null)
        {
            return;
        }

        roomLootRoller.OnLootRolled += HandleLootRolled;
    }

    private void OnDisable()
    {
        if (roomLootRoller == null)
        {
            return;
        }

        roomLootRoller.OnLootRolled -= HandleLootRolled;
    }

    private void HandleLootRolled(LootEntry rolledLoot)
    {
        if (rolledLoot == null)
        {
            return;
        }

        if (rolledLoot.RewardType == LootRewardType.None)
        {
            return;
        }

        if (roomContext == null)
        {
            return;
        }

        if (dropPool == null)
        {
            return;
        }

        if (roomMarkerTilemap == null)
        {
            return;
        }

        bool foundSmallPickupMarker = roomMarkerTilemap.TryGetSmallPickupWorldPosition(out Vector3 spawnPosition);

        if (!foundSmallPickupMarker)
        {
            return;
        }

        PooledDrop spawnedDrop = dropPool.GetDrop(spawnPosition);

        if (spawnedDrop == null)
        {
            return;
        }

        spawnedDrop.Configure(rolledLoot);
    }

    public void SetDropPool(DropPool newDropPool)
    {
        dropPool = newDropPool;
    }
}