using UnityEngine;

public class RoomItemRewardSpawner : MonoBehaviour
{
    [Header("Room References")]
    [SerializeField] private RoomContext roomContext;
    [SerializeField] private RoomController roomController;
    [SerializeField] private ItemRewardRoller itemRewardRoller;

    [Header("Item Reward")]
    [SerializeField] private ItemPedestal itemPedestalPrefab;

    [SerializeField] private RoomMarkerTilemap roomMarkerTilemap;

    private ItemPedestal spawnedItemPedestal;

    private void OnEnable()
    {
        if (itemRewardRoller != null)
        {
            itemRewardRoller.OnItemRolled += HandleItemRolled;
        }

        if (roomController != null)
        {
            roomController.OnRoomCleared += HandleRoomCleared;
        }
    }

    private void OnDisable()
    {
        if (itemRewardRoller != null)
        {
            itemRewardRoller.OnItemRolled -= HandleItemRolled;
        }

        if (roomController != null)
        {
            roomController.OnRoomCleared -= HandleRoomCleared;
        }
    }

    public void InitializeRoomItemReward()
    {
        if (roomContext == null)
        {
            return;
        }

        if (roomContext.RoomType == RoomType.Reward)
        {
            RollItemReward();
        }
    }

    private void HandleRoomCleared()
    {
        if (roomContext == null)
        {
            return;
        }

        if (roomContext.RoomType != RoomType.Boss)
        {
            return;
        }

        RollItemReward();
    }

    private void RollItemReward()
    {
        if (itemRewardRoller == null)
        {
            return;
        }

        itemRewardRoller.RollItemReward();
    }

    private void HandleItemRolled(ItemData rolledItem)
    {
        if (rolledItem == null)
        {
            return;
        }

        if (spawnedItemPedestal != null)
        {
            return;
        }

        if (roomContext == null)
        {
            return;
        }

        if (itemPedestalPrefab == null)
        {
            return;
        }

        if (roomMarkerTilemap == null)
        {
            return;
        }

        bool foundItemRewardMarker = roomMarkerTilemap.TryGetItemRewardWorldPosition(out Vector3 spawnPosition);

        if (!foundItemRewardMarker)
        {
            return;
        }
        spawnedItemPedestal = Instantiate( itemPedestalPrefab, spawnPosition,Quaternion.identity);

        spawnedItemPedestal.Configure(rolledItem);
    }
}