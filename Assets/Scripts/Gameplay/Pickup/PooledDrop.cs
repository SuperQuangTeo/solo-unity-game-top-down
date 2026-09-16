using DG.Tweening;
using UnityEngine;

public class PooledDrop : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite keySprite;
    [SerializeField] private Sprite bombSprite;

    private DropPool ownerPool;

    private Vector3 originalScale;

    private LootRewardType rewardType;

    private int rewardAmount;

    public LootRewardType RewardType => rewardType;
    public int RewardAmount => rewardAmount;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetOwnerPool(DropPool newOwnerPool)
    {
        ownerPool = newOwnerPool;
    }

    public void Configure(LootEntry lootEntry)
    {
        if (lootEntry == null)
        {
            return;
        }

        rewardType = lootEntry.RewardType;
        rewardAmount = lootEntry.Amount;

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        switch (rewardType)
        {
            case LootRewardType.Coin:
                spriteRenderer.sprite = coinSprite;
                break;

            case LootRewardType.Heart:
                spriteRenderer.sprite = heartSprite;
                break;

            case LootRewardType.Key:
                spriteRenderer.sprite = keySprite;
                break;

            case LootRewardType.Bomb:
                spriteRenderer.sprite = bombSprite;
                break;

            default:
                spriteRenderer.sprite = null;
                break;
        }
    }

    public void ResetForReuse()
    {
        transform.DOKill();

        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;

        rewardType = LootRewardType.None;
        rewardAmount = 0;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = null;
        }
    }

    public void ReturnToPool()
    {
        if (ownerPool == null)
        {
            return;
        }

        ownerPool.ReleaseDrop(this);
    }

    private void OnDisable()
    {
        transform.DOKill();

        transform.localScale = originalScale;
    }
}