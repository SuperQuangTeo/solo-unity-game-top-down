using System.Collections.Generic;
using UnityEngine;

public class FloorTransitionPrototype : MonoBehaviour
{
    [Header("Prototype References")]
    [SerializeField] private EdgarDungeonGenerator edgarDungeonGenerator;

    [Header("Pool Cleanup")]
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private List<ProjectilePool> projectilePools = new List<ProjectilePool>();
    [SerializeField] private DropPool dropPool;

    [Header("Floor Order")]
    [SerializeField]
    private List<FloorEdgarConfig> floorEdgarConfigs = new List<FloorEdgarConfig>();

    [SerializeField, Min(0)] private int initialFloorIndex;

    private int currentFloorIndex;
    private bool hasRequestedFloorTransition;

    public bool HasRequestedFloorTransition => hasRequestedFloorTransition;
    public int CurrentFloorIndex => currentFloorIndex;

    private void Awake()
    {
        currentFloorIndex = initialFloorIndex;

        ValidateInitialFloorConfiguration();
    }

    public void RequestNextFloor()
    {
        if (hasRequestedFloorTransition)
        {
            return;
        }

        if (!TryPrepareNextFloorConfig())
        {
            return;
        }

        hasRequestedFloorTransition = true;

        GeneratePreparedFloor();
    }

    private void GeneratePreparedFloor()
    {
        ReleaseRuntimeObjectsToPools();

        bool wasGenerated =
            edgarDungeonGenerator.TryGenerateDungeon();

        if (!wasGenerated)
        {
            hasRequestedFloorTransition = false;
            return;
        }

        Debug.Log($"{currentFloorIndex + 1}: " +
            $"{edgarDungeonGenerator.CurrentFloorEdgarConfig.name}.", this);

        hasRequestedFloorTransition = false;
    }

    private void ReleaseRuntimeObjectsToPools()
    {
        if (enemyPool != null)
        {
            enemyPool.ReleaseAllRentedEnemies();
        }

        foreach (ProjectilePool projectilePool in projectilePools)
        {
            if (projectilePool == null)
            {
                continue;
            }

            projectilePool.ReleaseAllRentedProjectiles();
        }

        if (dropPool != null)
        {
            dropPool.ReleaseAllRentedDrops();
        }
    }

    private bool TryPrepareNextFloorConfig()
    {
        int nextFloorIndex = currentFloorIndex + 1;

        if (floorEdgarConfigs == null ||
            floorEdgarConfigs.Count == 0)
        {
            Debug.LogError("floorEdgarConfigs.Count == 0", this);

            return false;
        }

        if (nextFloorIndex >= floorEdgarConfigs.Count)
        {
            Debug.Log("(nextFloorIndex >= floorEdgarConfigs.Count", this);

            return false;
        }

        if (edgarDungeonGenerator == null)
        {
            return false;
        }

        FloorEdgarConfig nextFloorConfig = floorEdgarConfigs[nextFloorIndex];

        if (nextFloorConfig == null)
        {
            Debug.LogError($"Floor config at index {nextFloorIndex} is null.", this);

            return false;
        }

        bool wasConfigAssigned = edgarDungeonGenerator.SetFloorEdgarConfig(nextFloorConfig);

        if (!wasConfigAssigned)
        {
            return false;
        }

        currentFloorIndex = nextFloorIndex;

        return true;
    }

    private void ValidateInitialFloorConfiguration()
    {
        if (floorEdgarConfigs == null || floorEdgarConfigs.Count == 0)
        {
            return;
        }

        if (initialFloorIndex >= floorEdgarConfigs.Count)
        {
            Debug.LogWarning("initialFloorIndex >= floorEdgarConfigs.Count", this);

            return;
        }

        if (edgarDungeonGenerator == null)
        {
            return;
        }

        FloorEdgarConfig expectedInitialConfig = floorEdgarConfigs[initialFloorIndex];

        if (expectedInitialConfig == edgarDungeonGenerator.CurrentFloorEdgarConfig)
        {
            return;
        }
    }
}