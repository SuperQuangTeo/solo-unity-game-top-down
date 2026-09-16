using System;
using UnityEngine;

public class RoomLootRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomController roomController;

    [Header("Loot Data")]
    [SerializeField] private LootTable roomClearLootTable;

    public event Action<LootEntry> OnLootRolled;

    private void OnEnable()
    {
        if (roomController == null)
        {
            return;
        }
        roomController.OnRoomCleared += HandleRoomCleared;
    }

    private void OnDisable()
    {
        if (roomController == null)
        {
            return;
        }
        roomController.OnRoomCleared -= HandleRoomCleared;
    }

    private void HandleRoomCleared()
    {
        if (roomClearLootTable == null)
        {
            return;
        }

        LootEntry rolledLoot = roomClearLootTable.RollLoot();

        if (rolledLoot == null)
        {
            return;
        }

        OnLootRolled?.Invoke(rolledLoot);

        Debug.Log(
            $"Room '{gameObject.name}' rolled: " +
            $"{rolledLoot.RewardType} - Amount: {rolledLoot.Amount}",
            this
        );
    }
}