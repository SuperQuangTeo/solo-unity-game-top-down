using System;
using UnityEngine;

[Serializable]
public class LootEntry
{
    [Header("Reward")]
    [SerializeField] private LootRewardType rewardType;

    [Header("Roll Weight")]
    [SerializeField] private int weight = 1;

    [Header("Reward Amount")]
    [SerializeField] private int amount = 1;

    public LootRewardType RewardType => rewardType;
    public int Weight => weight;
    public int Amount => amount;
}