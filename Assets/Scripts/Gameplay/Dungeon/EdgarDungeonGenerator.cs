using Edgar.Unity;
using System;
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
    public int CurrentGenerationSeed { get; private set; }

    private readonly DynamicLevelGraphBuilder dynamicLevelGraphBuilder = new DynamicLevelGraphBuilder();

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

        if (!TryApplyFloorConfiguration())
        {
            return false;
        }

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
        if (floorEdgarConfig.BaseLevelGraph == null)
        {
            Debug.LogError(
                "FloorEdgarConfig chưa được gán Base Level Graph.",
                this
            );

            return false;
        }
        if (floorEdgarConfig.DynamicLevelGraphConfig == null)
        {
            Debug.LogError(
                "FloorEdgarConfig chưa được gán Dynamic Level Graph Config.",
                this
            );

            return false;
        }
        return true;
    }

    private bool TryApplyFloorConfiguration()
    {
        FloorData floorData = floorEdgarConfig.FloorData;

        if (floorData == null)
        {
            Debug.LogError("FloorEdgarConfig chưa được gán FloorData.", this);
            return false;
        }

        CurrentGenerationSeed = GetGenerationSeed(floorData);

        try
        {
            LevelGraph runtimeLevelGraph = dynamicLevelGraphBuilder.Build(
                floorEdgarConfig.BaseLevelGraph,
                floorEdgarConfig.DynamicLevelGraphConfig,
                CurrentGenerationSeed
            );

            dungeonGenerator.FixedLevelGraphConfig.LevelGraph = runtimeLevelGraph;
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Không thể build Dynamic Level Graph: {exception.Message}",
                this
            );

            return false;
        }

        // Builder và Edgar dùng chung seed để cùng một seed tạo lại cùng topology/layout.
        dungeonGenerator.UseRandomSeed = false;
        dungeonGenerator.RandomGeneratorSeed = CurrentGenerationSeed;

        Debug.Log(
            $"Dynamic Level Graph đã build với seed: {CurrentGenerationSeed}",
            this
        );

        return true;
    }

    private int GetGenerationSeed(FloorData floorData)
    {
        return floorData.UseRandomSeed
            ? Guid.NewGuid().GetHashCode()
            : floorData.FixedSeed;
    }

    [ContextMenu("Test Generate Dungeon")]
    private void TestGenerateDungeon()
    {
        GenerateDungeon();
    }
}
