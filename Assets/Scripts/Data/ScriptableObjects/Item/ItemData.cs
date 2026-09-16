using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData",menuName = "Game Data/Item/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId;

    [SerializeField] private string displayName;

    [Header("Visual")]
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
}