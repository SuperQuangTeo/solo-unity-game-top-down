using System;
using UnityEngine;

public class ItemRewardRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomContext roomContext;

    private bool hasRolledItem;

    public event Action<ItemData> OnItemRolled;

    public void RollItemReward()
    {
        if (hasRolledItem)
        {
            return;
        }

        if (roomContext == null)
        {
            return;
        }

        ItemPoolData itemPoolData = roomContext.ItemPoolData;

        if (itemPoolData == null)
        {
            return;
        }

        ItemData rolledItem = itemPoolData.RollItem();

        if (rolledItem == null)
        {
            return;
        }

        hasRolledItem = true;

        OnItemRolled?.Invoke(rolledItem);

        Debug.Log(
            $"Room '{gameObject.name}' rolled Item: {rolledItem.DisplayName}",
            this
        );

    }

    [ContextMenu("Test Roll Item Reward")]
    private void TestRollItemReward()
    {
        RollItemReward();
    }
}