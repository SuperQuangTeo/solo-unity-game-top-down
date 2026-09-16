using System;
using UnityEngine;

[Serializable]
public class ItemPoolEntry
{
    [Header("Item")]
    [SerializeField] private ItemData itemData;

    [Header("Roll Weight")]
    [SerializeField] private int weight = 1;

    public ItemData ItemData => itemData;
    public int Weight => weight;
}