using Edgar.Unity;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloorEdgarConfig",menuName = "Dungeon/Floor Edgar Config")]
public class FloorEdgarConfig : ScriptableObject
{
    [Header("Floor Data")]
    [SerializeField] private FloorData floorData;

    [Header("Edgar Configuration")]
    [SerializeField] private List<LevelGraph> levelGraphs;

    public FloorData FloorData => floorData;

    public List<LevelGraph> LevelGraphs => levelGraphs;
    private void OnValidate()
    {
        if (floorData == null || levelGraphs == null || levelGraphs.Count == 0)
        {
            return;
        }

        ValidateLevelGraphs();
        ValidateRequiredRoomTypes();
    }

    private void ValidateLevelGraphs()
    {
        for (int i = 0; i < levelGraphs.Count; i++)
        {
            LevelGraph levelGraph = levelGraphs[i];

            if (levelGraph == null)
            {
                Debug.LogWarning(
                    $"Floor '{floorData.name}' has an empty Level Graph at index {i}.",
                    this
                );
            }
        }
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