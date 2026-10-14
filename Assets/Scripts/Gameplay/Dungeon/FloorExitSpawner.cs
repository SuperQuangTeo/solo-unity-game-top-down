using UnityEngine;

public class FloorExitSpawner : MonoBehaviour
{
    [Header("Room References")]
    [SerializeField] private RoomContext roomContext;
    [SerializeField] private RoomController roomController;

    [Header("Floor Exit")]
    [SerializeField] private FloorExit floorExitPrefab;

    [SerializeField] private RoomMarkerTilemap roomMarkerTilemap;
    private FloorExit spawnedFloorExit;
    private FloorTransitionPrototype floorTransitionPrototype;

    private void OnEnable()
    {
        if (roomController == null)
        {
            return;
        }

        roomController.OnRoomCleared += HandleRoomCleared;
    }

    private void OnDisable()
    {
        if (spawnedFloorExit != null)
        {
            spawnedFloorExit.OnPlayerEnteredExit -= HandlePlayerEnteredExit;
        }
        if (roomController == null)
        {
            return;
        }

        roomController.OnRoomCleared -= HandleRoomCleared;
    }

    private void HandleRoomCleared()
    {
        if (roomContext == null)
        {
            return;
        }

        if (roomContext.RoomType != RoomType.Boss)
        {
            return;
        }

        SpawnFloorExit();
    }

    private void SpawnFloorExit()
    {
        if (spawnedFloorExit != null)
        {
            return;
        }

        if (roomMarkerTilemap == null)
        {
            return;
        }

        if (floorExitPrefab == null)
        {
            return;
        }

        bool foundFloorExitMarker = roomMarkerTilemap.TryGetFloorExitWorldPosition(out Vector3 spawnPosition);

        if (!foundFloorExitMarker)
        {
            return;
        }

        spawnedFloorExit = Instantiate(floorExitPrefab, spawnPosition, Quaternion.identity, transform);

        spawnedFloorExit.OnPlayerEnteredExit += HandlePlayerEnteredExit;
    }

    public void SetFloorTransitionPrototype(FloorTransitionPrototype newFloorTransitionPrototype)
    {
        floorTransitionPrototype = newFloorTransitionPrototype;
    }

    private void HandlePlayerEnteredExit()
    {
        if (floorTransitionPrototype == null)
        {
            return;
        }

        floorTransitionPrototype.RequestNextFloor();
    }
}