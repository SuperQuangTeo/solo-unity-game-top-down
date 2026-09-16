using UnityEngine;

[CreateAssetMenu(
    fileName = "NewLootTable",
    menuName = "Game Data/Loot/Loot Table"
)]
public class LootTable : ScriptableObject
{
    [Header("Loot Entries")]
    [SerializeField] private LootEntry[] lootEntries;

    public LootEntry RollLoot()
    {
        if (lootEntries == null || lootEntries.Length == 0)
        {
            return null;
        }

        int totalWeight = CalculateTotalWeight();

        if (totalWeight <= 0)
        {
            return null;
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (LootEntry lootEntry in lootEntries)
        {
            if (lootEntry == null)
            {
                continue;
            }

            if (lootEntry.Weight <= 0)
            {
                continue;
            }

            if (randomValue < lootEntry.Weight)
            {
                return lootEntry;
            }

            randomValue -= lootEntry.Weight;
        }

        return null;
    }

    private int CalculateTotalWeight()
    {
        int totalWeight = 0;

        foreach (LootEntry lootEntry in lootEntries)
        {
            if (lootEntry == null)
            {
                continue;
            }

            if (lootEntry.Weight <= 0)
            {
                continue;
            }

            totalWeight += lootEntry.Weight;
        }

        return totalWeight;
    }
}