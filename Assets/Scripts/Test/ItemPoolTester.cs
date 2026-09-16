using UnityEngine;

public class ItemPoolTester : MonoBehaviour
{
    [Header("Item Pool")]
    [SerializeField] private ItemPoolData itemPoolData;

    [Header("Test Settings")]
    [SerializeField] private int testRollCount = 1000;

    [ContextMenu("Test Roll Once")]
    private void TestRollOnce()
    {
        if (itemPoolData == null)
        {

            return;
        }

        ItemData rolledItem = itemPoolData.RollItem();

        if (rolledItem == null)
        {


            return;
        }

        Debug.Log(
            $"Rolled Item: {rolledItem.DisplayName}",
            this
        );
    }

    [ContextMenu("Test Roll Distribution")]
    private void TestRollDistribution()
    {
        if (itemPoolData == null)
        {


            return;
        }

        if (testRollCount <= 0)
        {


            return;
        }

        int itemACount = 0;
        int itemBCount = 0;
        int itemCCount = 0;

        for (int rollIndex = 0; rollIndex < testRollCount; rollIndex++)
        {
            ItemData rolledItem = itemPoolData.RollItem();

            if (rolledItem == null)
            {
                continue;
            }

            if (rolledItem.ItemId == "magic_mushroom")
            {
                itemACount++;
            }
            else if (rolledItem.ItemId == "magic_mushroom_1")
            {
                itemBCount++;
            }
            else if (rolledItem.ItemId == "magic_mushroom_2")
            {
                itemCCount++;
            }
        }

        Debug.Log(
            $"Item Pool Test ({testRollCount} rolls)\n" +
            $"Item A: {itemACount}\n" +
            $"Item B: {itemBCount}\n" +
            $"Item C: {itemCCount}",
            this
        );
    }
}