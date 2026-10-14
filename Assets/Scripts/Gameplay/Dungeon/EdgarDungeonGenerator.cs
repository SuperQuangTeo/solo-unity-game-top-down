using Edgar.Unity;
using System.Collections.Generic;
using UnityEngine;

public class EdgarDungeonGenerator : MonoBehaviour
{
    [Header("Floor Configuration")]
    [SerializeField]
    private FloorEdgarConfig floorEdgarConfig;

    [Header("Edgar Reference")]
    [SerializeField]
    private DungeonGeneratorGrid2D dungeonGenerator;

    public FloorEdgarConfig CurrentFloorEdgarConfig => floorEdgarConfig;

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        TryGenerateDungeon();
    }

    public bool TryGenerateDungeon()
    {
        if (!ValidateReferences())
        {
            return false;
        }

        ApplyFloorConfiguration();

        dungeonGenerator.Generate();

        return true;
    }

    public bool SetFloorEdgarConfig(FloorEdgarConfig newFloorEdgarConfig)
    {
        if (newFloorEdgarConfig == null)
        {
            Debug.LogError(
                "cannot change Floor cause FloorEdgarConfig is null.",
                this
            );

            return false;
        }

        floorEdgarConfig = newFloorEdgarConfig;

        Debug.Log(
            $"EdgarDungeonGenerator recieved Floor config: " +
            $"{floorEdgarConfig.name}",
            this
        );

        return true;
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
        if (floorEdgarConfig.LevelGraphs == null ||
           floorEdgarConfig.LevelGraphs.Count == 0)
        {
            Debug.LogError(
                "FloorEdgarConfig does not contain any Level Graph.",
                this
            );

            return false;
        }
        return true;
    }

    private void ApplyFloorConfiguration()
    {
        FloorData floorData = floorEdgarConfig.FloorData;

        LevelGraph selectedLevelGraph = GetRandomLevelGraph();

        dungeonGenerator.FixedLevelGraphConfig.LevelGraph = selectedLevelGraph;
        dungeonGenerator.UseRandomSeed = floorData.UseRandomSeed;
        dungeonGenerator.RandomGeneratorSeed = floorData.FixedSeed;
    }

    private LevelGraph GetRandomLevelGraph()
    {
        List<LevelGraph> levelGraphs = floorEdgarConfig.LevelGraphs;

        int randomIndex = Random.Range(0, levelGraphs.Count);

        LevelGraph selectedLevelGraph = levelGraphs[randomIndex];

        Debug.Log(
            $"Selected Level Graph: {selectedLevelGraph.name}",
            this
        );

        return selectedLevelGraph;
    }

    [ContextMenu("Test Generate Dungeon")]
    private void TestGenerateDungeon()
    {
        GenerateDungeon();
    }
}