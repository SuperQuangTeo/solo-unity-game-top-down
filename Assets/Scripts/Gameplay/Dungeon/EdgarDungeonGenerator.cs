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
}