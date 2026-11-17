using Edgar.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorEdgarConfig",menuName = "Dungeon/Floor Edgar Config")]
public class FloorEdgarConfig : ScriptableObject
{
    [Header("Floor Data")]
    [SerializeField] private FloorData floorData;

    [Header("Edgar Dynamic Graph")]
    [SerializeField] private LevelGraph baseLevelGraph;
    [SerializeField] private DynamicLevelGraphConfig dynamicLevelGraphConfig;

    public FloorData FloorData => floorData;
    public LevelGraph BaseLevelGraph => baseLevelGraph;
    public DynamicLevelGraphConfig DynamicLevelGraphConfig => dynamicLevelGraphConfig;

    private void OnValidate()
    {
        if (floorData == null)
        {
            return;
        }

        ValidateRequiredRoomTypes();
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
