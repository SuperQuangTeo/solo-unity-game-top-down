using UnityEngine;

public class ItemPedestal : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer itemSpriteRenderer;

    private ItemData itemData;

    public ItemData ItemData => itemData;

    public void Configure(ItemData newItemData)
    {
        if (newItemData == null)
        {
            return;
        }

        itemData = newItemData;

        if (itemSpriteRenderer == null)
        {
            return;
        }

        itemSpriteRenderer.sprite = itemData.Icon;
    }
}