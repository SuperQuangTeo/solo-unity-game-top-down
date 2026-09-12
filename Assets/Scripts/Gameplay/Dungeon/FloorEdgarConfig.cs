using Edgar.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorEdgarConfig",menuName = "Dungeon/Floor Edgar Config")]
public class FloorEdgarConfig : ScriptableObject
{
    [Header("Floor Data")]
    [SerializeField] private FloorData floorData;

    [Header("Edgar Configuration")]
    [SerializeField] private LevelGraph levelGraph;

    public FloorData FloorData => floorData;

    public LevelGraph LevelGraph => levelGraph;
    private void OnValidate()
    {
        if (floorData == null || levelGraph == null)
        {
            return;
        }

        ValidateRoomCount();
        ValidateRequiredRoomTypes();
    }


    private void ValidateRoomCount()
    {
        int graphRoomCount = levelGraph.Rooms.Count;

        if (floorData.RoomCount == graphRoomCount)
        {
            return;
        }
        Debug.LogWarning(
            $"Floor '{floorData.name}' required {floorData.RoomCount} room, " +
            $"but Level Graph '{levelGraph.name}' Actually {graphRoomCount} room.",
            this
        );
    }

    private void ValidateRequiredRoomTypes()
    {
        if (!floorData.HasRoomType(RoomType.Start))
        {
            Debug.LogWarning(
                $"Floor '{floorData.name}' has no RoomType.Start.",
                this
            );
        }

        if (!floorData.HasRoomType(RoomType.Boss))
        {
            Debug.LogWarning(
                $"Floor '{floorData.name}' has no RoomType.Boss.",
                this
            );
        }
    }
}