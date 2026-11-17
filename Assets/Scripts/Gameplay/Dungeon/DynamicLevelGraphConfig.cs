using UnityEngine;

[CreateAssetMenu(fileName = "NewDynamicLevelGraphConfig", menuName = "Dungeon/Dynamic Level Graph Config")]
public class DynamicLevelGraphConfig : ScriptableObject
{
    [Header("Main Path - bao gồm Start và Boss")]
    [SerializeField, Min(2)] private int minimumMainPathRoomCount = 5;
    [SerializeField, Min(2)] private int maximumMainPathRoomCount = 7;

    [Header("Branches - không tính Room cha trên Main Path")]
    [SerializeField, Min(0)] private int minimumBranchCount = 0;
    [SerializeField, Min(0)] private int maximumBranchCount = 2;
    [SerializeField, Min(1)] private int minimumBranchRoomCount = 1;
    [SerializeField, Min(1)] private int maximumBranchRoomCount = 3;

    [Header("Branch End Weights")]
    [SerializeField, Min(0)] private int rewardEndWeight = 1;
    [SerializeField, Min(0)] private int shopEndWeight = 1;
    [SerializeField, Min(0)] private int combatDeadEndWeight = 1;

    public int MinimumMainPathRoomCount => minimumMainPathRoomCount;
    public int MaximumMainPathRoomCount => maximumMainPathRoomCount;
    public int MinimumBranchCount => minimumBranchCount;
    public int MaximumBranchCount => maximumBranchCount;
    public int MinimumBranchRoomCount => minimumBranchRoomCount;
    public int MaximumBranchRoomCount => maximumBranchRoomCount;

    public int RewardEndWeight => rewardEndWeight;
    public int ShopEndWeight => shopEndWeight;
    public int CombatDeadEndWeight => combatDeadEndWeight;

    public int GetBranchEndWeight(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.Reward => rewardEndWeight,
            RoomType.Shop => shopEndWeight,
            RoomType.Combat => combatDeadEndWeight,
            _ => 0
        };
    }

    public bool TryValidate(out string errorMessage)
    {
        if (minimumMainPathRoomCount > maximumMainPathRoomCount)
        {
            errorMessage = "Minimum Main Path Room Count không được lớn hơn Maximum Main Path Room Count.";
            return false;
        }

        if (minimumBranchCount > maximumBranchCount)
        {
            errorMessage = "Minimum Branch Count không được lớn hơn Maximum Branch Count.";
            return false;
        }

        if (minimumBranchRoomCount > maximumBranchRoomCount)
        {
            errorMessage = "Minimum Branch Room Count không được lớn hơn Maximum Branch Room Count.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
