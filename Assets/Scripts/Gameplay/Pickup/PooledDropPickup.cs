using UnityEngine;

public class PooledDropPickup : MonoBehaviour
{
    [Header("Drop Reference")]
    [SerializeField] private PooledDrop pooledDrop;

    private bool isCollected;

    private void OnEnable()
    {
        isCollected = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected)
        {
            return;
        }

        if (pooledDrop == null)
        {
            return;
        }

        if (!other.TryGetComponent<PlayerResources>(out PlayerResources playerResources))
        {
            return;
        }

        bool wasCollected = TryApplyReward(other, playerResources);

        if (!wasCollected)
        {
            return;
        }

        isCollected = true;

        pooledDrop.ReturnToPool();
    }

    private bool TryApplyReward(Collider2D playerCollider, PlayerResources playerResources)
    {
        int rewardAmount = pooledDrop.RewardAmount;

        if (rewardAmount <= 0)
        {
            return false;
        }

        switch (pooledDrop.RewardType)
        {
            case LootRewardType.Coin:
                playerResources.AddCoins(rewardAmount);
                return true;

            case LootRewardType.Key:
                playerResources.AddKeys(rewardAmount);
                return true;

            case LootRewardType.Bomb:
                playerResources.AddBombs(rewardAmount);
                return true;

            case LootRewardType.Heart:
                return TryCollectHeart(playerCollider, rewardAmount);

            default:
                return false;
        }
    }

    private bool TryCollectHeart(Collider2D playerCollider, int healAmount)
    {
        if (!playerCollider.TryGetComponent<PlayerHealth>(
                out PlayerHealth playerHealth))
        {
            return false;
        }

        return playerHealth.TryHeal(healAmount);
    }
}