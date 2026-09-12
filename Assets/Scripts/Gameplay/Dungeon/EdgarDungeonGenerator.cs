using Edgar.Unity;
using UnityEngine;

public class EdgarDungeonGenerator : MonoBehaviour
{
    [Header("Floor Configuration")]
    [SerializeField]
    private FloorEdgarConfig floorEdgarConfig;

    [Header("Edgar Reference")]
    [SerializeField]
    private DungeonGeneratorGrid2D dungeonGenerator;

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        if (!ValidateReferences())
        {
            return;
        }
        ApplyFloorConfiguration();

        dungeonGenerator.Generate();
    }

    [ContextMenu("Test Generate Dungeon")]
    private void TestGenerateDungeon()
    {
        GenerateDungeon();
    }

    private bool ValidateReferences()
    {
        if (floorEdgarConfig == null)
        {
            Debug.LogError(
                "EdgarDungeonGenerator is not attached to FloorEdgarConfig.",
                this
            );

            return false;
        }
        if (dungeonGenerator == null)
        {
            Debug.LogError(
                "EdgarDungeonGenerator is not attached to DungeonGeneratorGrid2D.",
                this
            );

            return false;
        }
        return true;
    }

    private void ApplyFloorConfiguration()
    {
        FloorData floorData = floorEdgarConfig.FloorData;

        dungeonGenerator.FixedLevelGraphConfig.LevelGraph = floorEdgarConfig.LevelGraph;
        dungeonGenerator.UseRandomSeed = floorData.UseRandomSeed;
        dungeonGenerator.RandomGeneratorSeed = floorData.FixedSeed;
    }
}