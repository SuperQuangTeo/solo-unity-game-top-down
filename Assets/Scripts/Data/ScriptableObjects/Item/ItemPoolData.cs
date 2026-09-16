using UnityEngine;

[CreateAssetMenu(fileName = "NewItemPool", menuName = "Game Data/Item/Item Pool")]
public class ItemPoolData : ScriptableObject
{
    [Header("Item Pool")]
    [SerializeField] private ItemPoolEntry[] itemEntries;

    public ItemData RollItem()
    {
        if (itemEntries == null || itemEntries.Length == 0)
        {
            return null;
        }

        int totalWeight = CalculateTotalWeight();

        if (totalWeight <= 0)
        {
            return null;
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (ItemPoolEntry itemEntry in itemEntries)
        {
            if (itemEntry == null)
            {
                continue;
            }

            if (itemEntry.ItemData == null)
            {
                continue;
            }

            if (itemEntry.Weight <= 0)
            {
                continue;
            }

            if (randomValue < itemEntry.Weight)
            {
                return itemEntry.ItemData;
            }

            randomValue -= itemEntry.Weight;
        }

        return null;
    }

    private int CalculateTotalWeight()
    {
        int totalWeight = 0;

        foreach (ItemPoolEntry itemEntry in itemEntries)
        {
            if (itemEntry == null)
            {
                continue;
            }

            if (itemEntry.ItemData == null)
            {
                continue;
            }

            if (itemEntry.Weight <= 0)
            {
                continue;
            }

            totalWeight += itemEntry.Weight;
        }

        return totalWeight;
    }
}