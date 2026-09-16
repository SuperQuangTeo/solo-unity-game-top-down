using UnityEngine;

public class LootTableTester : MonoBehaviour
{
    [Header("Loot Data")]
    [SerializeField] private LootTable lootTable;

    [Header("Test Settings")]
    [SerializeField] private int testRollCount = 1000;

    [ContextMenu("Test Roll Once")]
    private void TestRollOnce()
    {
        if (lootTable == null)
        {
            Debug.LogError(
                "LootTableTester has not attached LootTable.",
                this
            );

            return;
        }

        LootEntry rolledLoot = lootTable.RollLoot();

        if (rolledLoot == null)
        {
            Debug.LogWarning(
                "LootTable do not return LootEntry.",
                this
            );

            return;
        }

        Debug.Log(
            $"Roll Result: {rolledLoot.RewardType} - Amount: {rolledLoot.Amount}",
            this
        );
    }

    [ContextMenu("Test Roll Distribution")]
    private void TestRollDistribution()
    {
        if (lootTable == null)
        {
            Debug.LogError(
                "LootTableTester has not attached LootTable.",
                this
            );

            return;
        }

        if (testRollCount <= 0)
        {
            Debug.LogWarning(
                "Test Roll Count has to better than 0.",
                this
            );

            return;
        }

        int noneCount = 0;
        int coinCount = 0;
        int heartCount = 0;
        int keyCount = 0;
        int bombCount = 0;

        for (int rollIndex = 0; rollIndex < testRollCount; rollIndex++)
        {
            LootEntry rolledLoot = lootTable.RollLoot();

            if (rolledLoot == null)
            {
                continue;
            }

            switch (rolledLoot.RewardType)
            {
                case LootRewardType.None:
                    noneCount++;
                    break;

                case LootRewardType.Coin:
                    coinCount++;
                    break;

                case LootRewardType.Heart:
                    heartCount++;
                    break;

                case LootRewardType.Key:
                    keyCount++;
                    break;

                case LootRewardType.Bomb:
                    bombCount++;
                    break;
            }
        }

        Debug.Log(
            $"Loot Test ({testRollCount} rolls)\n" +
            $"None: {noneCount}\n" +
            $"Coin: {coinCount}\n" +
            $"Heart: {heartCount}\n" +
            $"Key: {keyCount}\n" +
            $"Bomb: {bombCount}",
            this
        );
    }
}