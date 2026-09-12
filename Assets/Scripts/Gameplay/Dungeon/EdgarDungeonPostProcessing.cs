using Edgar.Unity;
using System.Collections.Generic;
using UnityEngine;

public class EdgarDungeonPostProcessing : DungeonGeneratorPostProcessingComponentGrid2D
{
    [Header("Gameplay Door")]
    [SerializeField] private RoomDoor roomDoorPrefab;

    [Header("Minimap")]
    [SerializeField] private MinimapGenerator minimapGenerator;

    [Header("Room Transition")]
    [SerializeField] private RoomTransitionController roomTransitionController;

    [Header("Runtime Gameplay References")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private CameraController cameraController;

    public override void Run(DungeonGeneratorLevelGrid2D level)
    {
        List<RoomContext> generatedRoomContexts = new List<RoomContext>();

        int gameplayRoomCount = 0;
        int corridorCount = 0;
        int nextRoomId = 0;

        foreach (RoomInstanceGrid2D roomInstance in level.RoomInstances)
        {
            if (roomInstance.IsCorridor)
            {
                corridorCount++;
                continue;
            }
            bool wasRoomProcessed = ProcessGameplayRoom(roomInstance, nextRoomId);

            if (wasRoomProcessed)
            {
                RoomContext roomContext = roomInstance.RoomTemplateInstance.GetComponent<RoomContext>();
                generatedRoomContexts.Add(roomContext);

                gameplayRoomCount++;
                nextRoomId++;
            }
        }

        SetupRoomConnections(level);

        BuildMinimap(generatedRoomContexts);

        SetupInitialRoom(generatedRoomContexts);

        Debug.Log(
            $"Post Processing hoàn tất. " +
            $"Gameplay Rooms = {gameplayRoomCount}, " +
            $"Corridors = {corridorCount}.",
            this
        );
    }

    private bool ProcessGameplayRoom(RoomInstanceGrid2D roomInstance, int roomId)
    {
        GameObject roomTemplateInstance = roomInstance.RoomTemplateInstance;
        if (roomTemplateInstance == null)
        {
            Debug.LogWarning(
                "Không tìm thấy RoomTemplateInstance của gameplay room.",
                this
            );
            return false;
        }

        RoomController roomController = roomTemplateInstance.GetComponent<RoomController>();
        if (roomController == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không có RoomController.",
                roomTemplateInstance
            );
            return false;
        }

        roomController.SetRuntimeReferences(playerTransform, cameraController);

        Debug.Log(
            $"Room '{roomTemplateInstance.name}' " +
            $"có {roomInstance.Doors.Count} cửa đang được sử dụng.",
            roomTemplateInstance
        );

        LogDoorInformation(roomInstance, roomTemplateInstance);

        RoomContext roomContext = roomTemplateInstance.GetComponent<RoomContext>();
        if (roomContext == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không có RoomContext.",
                roomTemplateInstance
            );
            return false;
        }

        SetupRoomTransition(roomTemplateInstance);

        roomContext.SetRoomId(roomId);

        Vector3Int edgarRoomPosition = roomInstance.Position;

        Vector2Int roomLayoutPosition = new Vector2Int(edgarRoomPosition.x, edgarRoomPosition.y);

        roomContext.SetLayoutPosition(roomLayoutPosition);

        CollectRoomSpawnPoints(roomTemplateInstance, roomContext);

        CreateGameplayDoors(roomInstance, roomTemplateInstance, roomController);
        Debug.Log(
            $"Room '{roomTemplateInstance.name}' " +
            $"đã được gán RoomId = {roomContext.RoomId}, " +
            $"RoomType = {roomContext.RoomType}.",
            roomTemplateInstance
        );
        return true;
    }

    private void SetupInitialRoom(List<RoomContext> generatedRoomContexts)
    {
        if (roomTransitionController == null)
        {
            return;
        }

        RoomContext startRoom = null;

        // Tìm Room có RoomType.Start.
        // Không dựa vào RoomId vì RoomId 0 không được đảm bảo là Start Room.
        foreach (RoomContext roomContext in generatedRoomContexts)
        {
            if (roomContext == null)
            {
                continue;
            }

            if (roomContext.RoomType == RoomType.Start)
            {
                startRoom = roomContext;
                break;
            }
        }

        if (startRoom == null)
        {
            Debug.LogWarning(
                "Không tìm thấy Start Room để khởi tạo Room Transition.",
                this
            );

            return;
        }

        // Gán Start Room làm Current Room đầu tiên.
        // CameraController sẽ frame CameraAnchor của Start Room.
        roomTransitionController.SetInitialRoom(
            startRoom
        );
    }

    private void LogDoorInformation(RoomInstanceGrid2D roomInstance, GameObject roomTemplateInstance)
    {
        Grid roomGrid = roomTemplateInstance.GetComponentInChildren<Grid>();

        if (roomGrid == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không tìm thấy Grid.",
                roomTemplateInstance
            );
            return;
        }
        int doorIndex = 0;

        foreach (DoorInstanceGrid2D doorInstance in roomInstance.Doors)
        {
            doorIndex++;

            OrthogonalLine localDoorLine = doorInstance.DoorLine;

            Vector3Int localDoorFrom = localDoorLine.From;

            Vector3Int localDoorTo = localDoorLine.To;

            Vector3 worldDoorFrom = roomGrid.GetCellCenterWorld(localDoorFrom);

            Vector3 worldDoorTo = roomGrid.GetCellCenterWorld(localDoorTo);

            Vector3 worldDoorPosition = (worldDoorFrom + worldDoorTo) / 2f;

            Debug.Log(
                $"Room '{roomTemplateInstance.name}' | " +
                $"Door {doorIndex} | " +
                $"Local: {localDoorFrom} → {localDoorTo} | " +
                $"World: {worldDoorPosition} | " +
                $"Facing: {doorInstance.FacingDirection}",
                roomTemplateInstance
            );

            DrawDoorDebugMarker(worldDoorPosition);
        }
    }

    private void CreateGameplayDoors(RoomInstanceGrid2D roomInstance, GameObject roomTemplateInstance, RoomController roomController)
    {
        // Tìm child "Doors" của chính Room runtime này.
        Transform doorsContainer = roomTemplateInstance.transform.Find("Doors");

        // Nếu không có child Doors thì Room Template đang setup sai.
        if (doorsContainer == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không tìm thấy child 'Doors'.",
                roomTemplateInstance
            );

            return;
        }
        // Tìm Grid runtime của Room để đổi tọa độ Door sang World Position.
        Grid roomGrid = roomTemplateInstance.GetComponentInChildren<Grid>();

        // Không có Grid thì không thể xác định vị trí Door chính xác.
        if (roomGrid == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không tìm thấy Grid.",
                roomTemplateInstance
            );
            return;
        }
        // Nếu chưa gán prefab trong Inspector thì không thể tạo Door.
        if (roomDoorPrefab == null)
        {
            Debug.LogError(
                "EdgarDungeonPostProcessing chưa được gán RoomDoor prefab.",
                this
            );
            return;
        }

        // Danh sách tạm dùng để lưu tất cả Door vừa tạo của Room này.
        List<RoomDoor> createdRoomDoors = new List<RoomDoor>();

        // Duyệt qua tất cả Door thật mà Edgar đã sử dụng cho Room.
        foreach (DoorInstanceGrid2D doorInstance in roomInstance.Doors)
        {
            // Tính World Position chính xác của Door.
            Vector3 doorWorldPosition = GetDoorWorldPosition(doorInstance, roomGrid);

            // Tạo RoomDoor prefab tại vị trí cửa.
            RoomDoor createdRoomDoor = Instantiate(roomDoorPrefab, doorWorldPosition, Quaternion.identity, doorsContainer);

            // Thêm Door vừa tạo vào danh sách của Room.
            createdRoomDoors.Add(createdRoomDoor);
        }

        // Gửi toàn bộ Door runtime cho RoomController quản lý.
        roomController.SetRoomDoors(createdRoomDoors.ToArray());
    }

    private void CollectRoomSpawnPoints(GameObject roomTemplateInstance, RoomContext roomContext)
    {
        // Tìm tất cả EnemySpawnPoint nằm bên trong chính Room runtime này.
        EnemySpawnPoint[] enemySpawnPoints = roomTemplateInstance.GetComponentsInChildren<EnemySpawnPoint>(true);

        // Tìm RewardSpawnPoint đang active của Room.
        RewardSpawnPoint rewardSpawnPoint = roomTemplateInstance.GetComponentInChildren<RewardSpawnPoint>();

        PlayerSpawnPoint playerSpawnPoint = roomTemplateInstance.GetComponentInChildren<PlayerSpawnPoint>(true);

        // Gửi các Enemy Spawn Point cho RoomContext lưu lại.
        roomContext.SetEnemySpawnPoints(
            enemySpawnPoints
        );

        // Gửi Reward Spawn Point cho RoomContext lưu lại.
        roomContext.SetRewardSpawnPoint(
            rewardSpawnPoint
        );

        // Gửi PlayerSpawnPoint cho RoomContext lưu lại.
        roomContext.SetPlayerSpawnPoint(
            playerSpawnPoint
        );

        Debug.Log(
    $"Room '{roomTemplateInstance.name}' " +
    $"có {enemySpawnPoints.Length} Enemy Spawn Point. " +
    $"Reward Spawn Point = " +
    $"{(rewardSpawnPoint != null ? "Có" : "Không")}. " +
    $"Player Spawn Point = " +
    $"{(playerSpawnPoint != null ? "Có" : "Không")}.",
    roomTemplateInstance
);
    }

    private Vector3 GetDoorWorldPosition(DoorInstanceGrid2D doorInstance, Grid roomGrid)
    {
        // DoorLine nằm trong tọa độ local grid của Room Template.
        OrthogonalLine localDoorLine = doorInstance.DoorLine;

        // Lấy hai đầu của Door.
        Vector3Int localDoorFrom = localDoorLine.From;

        Vector3Int localDoorTo = localDoorLine.To;

        // Đổi hai đầu từ Cell Position sang World Position.
        Vector3 worldDoorFrom = roomGrid.GetCellCenterWorld(localDoorFrom);

        Vector3 worldDoorTo = roomGrid.GetCellCenterWorld(localDoorTo);

        // Trả về chính giữa toàn bộ Door.
        return (worldDoorFrom + worldDoorTo) / 2f;
    }

    private void BuildMinimap(List<RoomContext> generatedRoomContexts)
    {
        // Nếu scene hiện tại không có MinimapGenerator thì chỉ cảnh báo.
        if (minimapGenerator == null)
        {
            return;
        }

        // Chuyển List thành Array rồi gửi dữ liệu Room cho Minimap.
        minimapGenerator.BuildMinimap(generatedRoomContexts.ToArray());
    }

    private string GetConnectedRoomName(RoomInstanceGrid2D connectedRoomInstance)
    {
        if (connectedRoomInstance == null)
        {
            return "Không xác định";
        }
        if (connectedRoomInstance.RoomTemplateInstance != null)
        {
            return connectedRoomInstance.RoomTemplateInstance.name;
        }

        return "RoomInstance không có GameObject";
    }

    private void DrawDoorDebugMarker(Vector3 worldPosition)
    {
        float markerHalfSize = 0.35f;

        Debug.DrawLine(
            worldPosition + Vector3.left * markerHalfSize,
            worldPosition + Vector3.right * markerHalfSize,
            Color.red,
            100f
        );

        Debug.DrawLine(
            worldPosition + Vector3.down * markerHalfSize,
            worldPosition + Vector3.up * markerHalfSize,
            Color.red,
            100f
        );
    }

    private void SetupRoomConnections(DungeonGeneratorLevelGrid2D level)
    {
        foreach (RoomInstanceGrid2D roomInstance in level.RoomInstances)
        {
            if (roomInstance.IsCorridor)
            {
                continue;
            }

            GameObject roomTemplateInstance = roomInstance.RoomTemplateInstance;

            if (roomTemplateInstance == null)
            {
                continue;
            }

            RoomContext roomContext = roomTemplateInstance.GetComponent<RoomContext>();

            if (roomContext == null)
            {
                Debug.LogWarning(
                    $"Room '{roomTemplateInstance.name}' " +
                    $"không có RoomContext khi setup connection.",
                    roomTemplateInstance
                );

                continue;
            }

            List<RoomContext> connectedRoomContexts = new List<RoomContext>();

            foreach (DoorInstanceGrid2D doorInstance in roomInstance.Doors)
            {
                RoomInstanceGrid2D connectedGameplayRoomInstance = FindConnectedGameplayRoom(roomInstance, doorInstance.ConnectedRoomInstance);

                if (connectedGameplayRoomInstance == null)
                {
                    Debug.LogWarning(
                        $"Không tìm thấy gameplay Room ở đầu bên kia " +
                        $"của một Door trong Room '{roomTemplateInstance.name}'.",
                        roomTemplateInstance
                    );
                    continue;
                }

                GameObject connectedRoomGameObject = connectedGameplayRoomInstance.RoomTemplateInstance;

                if (connectedRoomGameObject == null)
                {
                    continue;
                }

                RoomContext connectedRoomContext = connectedRoomGameObject.GetComponent<RoomContext>();

                if (connectedRoomContext == null)
                {
                    Debug.LogWarning(
                        $"Connected Room '{connectedRoomGameObject.name}' " +
                        $"không có RoomContext.",
                        connectedRoomGameObject
                    );
                    continue;
                }

                if (!connectedRoomContexts.Contains(connectedRoomContext))
                {
                    connectedRoomContexts.Add(
                        connectedRoomContext
                    );
                }
            }

            roomContext.SetConnectedRooms(connectedRoomContexts.ToArray());
        }
    }

    private void SetupRoomTransition(GameObject roomTemplateInstance)
    {
        if (roomTransitionController == null)
        {
            Debug.LogWarning(
                "EdgarDungeonPostProcessing chưa được gán RoomTransitionController.",
                this
            );

            return;
        }

        RoomTrigger roomTrigger = roomTemplateInstance.GetComponentInChildren<RoomTrigger>(true);
        if (roomTrigger == null)
        {
            Debug.LogWarning(
                $"Room '{roomTemplateInstance.name}' không tìm thấy RoomTrigger.",
                roomTemplateInstance
            );

            return;
        }

        roomTrigger.SetRoomTransitionController(roomTransitionController);
    }

    private RoomInstanceGrid2D FindConnectedGameplayRoom(RoomInstanceGrid2D sourceRoomInstance, RoomInstanceGrid2D connectedRoomInstance)
    {
        if (connectedRoomInstance == null)
        {
            return null;
        }

        if (!connectedRoomInstance.IsCorridor)
        {
            return connectedRoomInstance;
        }

        foreach (DoorInstanceGrid2D corridorDoorInstance in connectedRoomInstance.Doors)
        {
            RoomInstanceGrid2D nextRoomInstance = corridorDoorInstance.ConnectedRoomInstance;

            if (nextRoomInstance == null)
            {
                continue;
            }

            if (nextRoomInstance == sourceRoomInstance)
            {
                continue;
            }

            if (!nextRoomInstance.IsCorridor)
            {
                return nextRoomInstance;
            }
        }
        return null;
    }
}