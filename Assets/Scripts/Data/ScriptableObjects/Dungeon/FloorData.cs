using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorData",menuName = "Dungeon/Floor Data")]
public class FloorData : ScriptableObject
{
    [Header("Floor Settings")]
    [SerializeField, Min(1)] private int roomCount = 6;
    [SerializeField, Min(1)] private int difficultyLevel = 1;


    [Header("Room Types")]
    [SerializeField] private RoomType[] roomTypes =
    {
        RoomType.Start,
        RoomType.Combat,
        RoomType.Reward,
        RoomType.Boss
    };

    [Header("Generation Seed")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int fixedSeed = 12345;

    public int RoomCount => roomCount;
    public int DifficultyLevel => difficultyLevel;
    public RoomType[] RoomTypes => roomTypes;
    public bool UseRandomSeed => useRandomSeed;
    public int FixedSeed => fixedSeed;

    public bool HasRoomType(RoomType roomType)
    {
        foreach (RoomType currentRoomType in roomTypes)
        {
            if (currentRoomType == roomType)
            {
                return true;
            }
        }
        return false;
    }
}