using Edgar.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLevelGraphBuilder
{
    private readonly List<UnityEngine.Object> createdRuntimeObjects = new List<UnityEngine.Object>();

    public LevelGraph Build(LevelGraph baseLevelGraph, DynamicLevelGraphConfig dynamicConfig, int generationSeed)
    {
        ReleasePreviousGraph();

        if (baseLevelGraph == null)
        {
            throw new ArgumentNullException(nameof(baseLevelGraph));
        }

        if (dynamicConfig == null)
        {
            throw new ArgumentNullException(nameof(dynamicConfig));
        }

        if (!dynamicConfig.TryValidate(out string validationError))
        {
            throw new InvalidOperationException(validationError);
        }

        System.Random random = new System.Random(generationSeed);
        Dictionary<RoomType, List<GameObject>> roomTemplatePools =
            CreateRoomTemplatePools(baseLevelGraph);

        ValidateRequiredRoomTemplatePools(roomTemplatePools, dynamicConfig);

        LevelGraph runtimeGraph = CreateRuntimeGraph(baseLevelGraph);
        List<Room> mainPathRooms = CreateMainPath(
            runtimeGraph,
            roomTemplatePools,
            dynamicConfig,
            random
        );
        List<RoomType> branchEndTypes =
            CreateBranches(
                runtimeGraph,
                roomTemplatePools,
                mainPathRooms,
                dynamicConfig,
                random
            );

        Debug.Log(
            $"Dynamic Graph | Main Path = {mainPathRooms.Count} Room | " +
            $"Branches = {branchEndTypes.Count} ({string.Join(", ", branchEndTypes)}) | " +
            $"Total Rooms = {runtimeGraph.Rooms.Count}"
        );

        return runtimeGraph;
    }

    public void ReleasePreviousGraph()
    {
        foreach (UnityEngine.Object createdRuntimeObject in createdRuntimeObjects)
        {
            if (createdRuntimeObject != null)
            {
                UnityEngine.Object.Destroy(createdRuntimeObject);
            }
        }

        createdRuntimeObjects.Clear();
    }

    private Dictionary<RoomType, List<GameObject>> CreateRoomTemplatePools(
        LevelGraph baseLevelGraph)
    {
        Dictionary<RoomType, List<GameObject>> roomTemplatePools =
            new Dictionary<RoomType, List<GameObject>>();

        foreach (RoomType roomType in Enum.GetValues(typeof(RoomType)))
        {
            roomTemplatePools.Add(roomType, new List<GameObject>());
        }

        foreach (RoomBase sourceRoom in baseLevelGraph.Rooms)
        {
            if (sourceRoom == null)
            {
                continue;
            }

            List<GameObject> roomTemplates = InputSetupUtils.GetRoomTemplates(
                sourceRoom,
                baseLevelGraph.DefaultRoomTemplateSets,
                baseLevelGraph.DefaultIndividualRoomTemplates
            );

            foreach (GameObject roomTemplate in roomTemplates)
            {
                AddTemplateToMatchingPool(roomTemplatePools, roomTemplate, sourceRoom);
            }
        }

        return roomTemplatePools;
    }

    private void AddTemplateToMatchingPool(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        GameObject roomTemplate,
        RoomBase sourceRoom)
    {
        if (roomTemplate == null)
        {
            return;
        }

        // RoomContext trên root prefab là nguồn xác định RoomType của project.
        RoomContext roomContext = roomTemplate.GetComponent<RoomContext>();
        if (roomContext == null)
        {
            Debug.LogWarning(
                $"Room Template '{roomTemplate.name}' của node " +
                $"'{sourceRoom.GetDisplayName()}' không có RoomContext nên bị bỏ qua."
            );
            return;
        }

        List<GameObject> matchingPool = roomTemplatePools[roomContext.RoomType];
        if (!matchingPool.Contains(roomTemplate))
        {
            matchingPool.Add(roomTemplate);
        }
    }

    private void ValidateRequiredRoomTemplatePools(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        DynamicLevelGraphConfig dynamicConfig)
    {
        if (!HasRoomTemplatePool(roomTemplatePools, RoomType.Start) ||
            !HasRoomTemplatePool(roomTemplatePools, RoomType.Combat) ||
            !HasRoomTemplatePool(roomTemplatePools, RoomType.Boss))
        {
            throw new InvalidOperationException(
                "Base Level Graph cần có ít nhất một Room Template hợp lệ cho Start, Combat và Boss."
            );
        }

        if (dynamicConfig.MaximumBranchCount > 0 &&
            GetTotalBranchEndWeight(roomTemplatePools, dynamicConfig) == 0)
        {
            throw new InvalidOperationException(
                "Base Level Graph chưa có Room Template hợp lệ cho Branch End đang có Weight lớn hơn 0."
            );
        }
    }

    private int GetTotalBranchEndWeight(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        DynamicLevelGraphConfig dynamicConfig)
    {
        int totalWeight = 0;

        foreach (RoomType roomType in new[] { RoomType.Reward, RoomType.Shop, RoomType.Combat })
        {
            if (HasRoomTemplatePool(roomTemplatePools, roomType))
            {
                totalWeight += dynamicConfig.GetBranchEndWeight(roomType);
            }
        }

        return totalWeight;
    }

    private bool HasRoomTemplatePool(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        RoomType roomType)
    {
        return roomTemplatePools.TryGetValue(roomType, out List<GameObject> roomTemplates) &&
               roomTemplates.Count > 0;
    }

    private bool TryGetRandomRoomTemplate(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        RoomType roomType,
        System.Random random,
        out GameObject roomTemplate)
    {
        if (!HasRoomTemplatePool(roomTemplatePools, roomType))
        {
            roomTemplate = null;
            return false;
        }

        List<GameObject> roomTemplates = roomTemplatePools[roomType];
        roomTemplate = roomTemplates[random.Next(0, roomTemplates.Count)];
        return true;
    }

    private LevelGraph CreateRuntimeGraph(LevelGraph baseLevelGraph)
    {
        LevelGraph runtimeGraph = CreateRuntimeObject<LevelGraph>();
        runtimeGraph.name = $"Runtime Dynamic Graph ({baseLevelGraph.name})";

        // Giữ lại cấu hình corridor/template chung của graph Edgar gốc.
        runtimeGraph.CorridorIndividualRoomTemplates =
            new List<GameObject>(baseLevelGraph.CorridorIndividualRoomTemplates);
        runtimeGraph.CorridorRoomTemplateSets =
            new List<RoomTemplatesSet>(baseLevelGraph.CorridorRoomTemplateSets);
        runtimeGraph.DefaultIndividualRoomTemplates =
            new List<GameObject>(baseLevelGraph.DefaultIndividualRoomTemplates);
        runtimeGraph.DefaultRoomTemplateSets =
            new List<RoomTemplatesSet>(baseLevelGraph.DefaultRoomTemplateSets);
        runtimeGraph.RoomType = baseLevelGraph.RoomType;
        runtimeGraph.ConnectionType = baseLevelGraph.ConnectionType;

        return runtimeGraph;
    }

    private List<Room> CreateMainPath(
        LevelGraph runtimeGraph,
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        DynamicLevelGraphConfig dynamicConfig,
        System.Random random)
    {
        int mainPathRoomCount = GetRandomInclusive(
            random,
            dynamicConfig.MinimumMainPathRoomCount,
            dynamicConfig.MaximumMainPathRoomCount
        );

        List<Room> mainPathRooms = new List<Room>();

        Room startRoom = CreateRoom(
            runtimeGraph,
            roomTemplatePools,
            random,
            RoomType.Start,
            "Start"
        );
        mainPathRooms.Add(startRoom);

        // Main path luôn có Start ở đầu và Boss ở cuối.
        for (int roomIndex = 1; roomIndex < mainPathRoomCount - 1; roomIndex++)
        {
            Room combatRoom = CreateRoom(
                runtimeGraph,
                roomTemplatePools,
                random,
                RoomType.Combat,
                $"Main Combat {roomIndex}"
            );

            ConnectRooms(runtimeGraph, mainPathRooms[mainPathRooms.Count - 1], combatRoom);
            mainPathRooms.Add(combatRoom);
        }

        Room bossRoom = CreateRoom(
            runtimeGraph,
            roomTemplatePools,
            random,
            RoomType.Boss,
            "Boss"
        );
        ConnectRooms(runtimeGraph, mainPathRooms[mainPathRooms.Count - 1], bossRoom);
        mainPathRooms.Add(bossRoom);

        return mainPathRooms;
    }

    private List<RoomType> CreateBranches(
        LevelGraph runtimeGraph,
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        List<Room> mainPathRooms,
        DynamicLevelGraphConfig dynamicConfig,
        System.Random random)
    {
        // Start và Boss không phải điểm gắn branch. Các Combat Room ở giữa là candidate.
        List<Room> branchCandidates = new List<Room>();
        for (int roomIndex = 1; roomIndex < mainPathRooms.Count - 1; roomIndex++)
        {
            branchCandidates.Add(mainPathRooms[roomIndex]);
        }

        Shuffle(branchCandidates, random);

        int requestedBranchCount = GetRandomInclusive(
            random,
            dynamicConfig.MinimumBranchCount,
            dynamicConfig.MaximumBranchCount
        );

        int actualBranchCount = Mathf.Min(requestedBranchCount, branchCandidates.Count);
        List<RoomType> branchEndTypes = new List<RoomType>();
        for (int branchIndex = 0; branchIndex < actualBranchCount; branchIndex++)
        {
            RoomType branchEndType = CreateSingleBranch(
                runtimeGraph,
                roomTemplatePools,
                branchCandidates[branchIndex],
                branchIndex + 1,
                dynamicConfig,
                random
            );

            branchEndTypes.Add(branchEndType);
        }

        return branchEndTypes;
    }

    private RoomType CreateSingleBranch(
        LevelGraph runtimeGraph,
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        Room branchParent,
        int branchNumber,
        DynamicLevelGraphConfig dynamicConfig,
        System.Random random)
    {
        int branchRoomCount = GetRandomInclusive(
            random,
            dynamicConfig.MinimumBranchRoomCount,
            dynamicConfig.MaximumBranchRoomCount
        );
        RoomType branchEndRoomType = GetBranchEndRoomType(
            roomTemplatePools,
            dynamicConfig,
            random
        );

        Room previousRoom = branchParent;
        for (int roomIndex = 0; roomIndex < branchRoomCount; roomIndex++)
        {
            bool isBranchEnd = roomIndex == branchRoomCount - 1;
            RoomType roomType = isBranchEnd
                ? branchEndRoomType
                : RoomType.Combat;

            string roomName = isBranchEnd
                ? $"Branch {branchNumber} {roomType} End"
                : $"Branch {branchNumber} Combat {roomIndex + 1}";

            Room branchRoom = CreateRoom(
                runtimeGraph,
                roomTemplatePools,
                random,
                roomType,
                roomName
            );
            ConnectRooms(runtimeGraph, previousRoom, branchRoom);
            previousRoom = branchRoom;
        }

        return branchEndRoomType;
    }

    private RoomType GetBranchEndRoomType(
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        DynamicLevelGraphConfig dynamicConfig,
        System.Random random)
    {
        int rewardWeight = HasRoomTemplatePool(roomTemplatePools, RoomType.Reward)
            ? dynamicConfig.GetBranchEndWeight(RoomType.Reward)
            : 0;
        int shopWeight = HasRoomTemplatePool(roomTemplatePools, RoomType.Shop)
            ? dynamicConfig.GetBranchEndWeight(RoomType.Shop)
            : 0;
        int combatWeight = HasRoomTemplatePool(roomTemplatePools, RoomType.Combat)
            ? dynamicConfig.GetBranchEndWeight(RoomType.Combat)
            : 0;

        int totalWeight = rewardWeight + shopWeight + combatWeight;

        int roll = random.Next(0, totalWeight);

        if (roll < rewardWeight)
        {
            return RoomType.Reward;
        }

        roll -= rewardWeight;
        if (roll < shopWeight)
        {
            return RoomType.Shop;
        }

        return RoomType.Combat;
    }

    private Room CreateRoom(
        LevelGraph runtimeGraph,
        Dictionary<RoomType, List<GameObject>> roomTemplatePools,
        System.Random random,
        RoomType roomType,
        string roomName)
    {
        if (!TryGetRandomRoomTemplate(
                roomTemplatePools,
                roomType,
                random,
                out GameObject roomTemplate))
        {
            throw new InvalidOperationException(
                $"Không tìm thấy Room Template cho RoomType.{roomType}."
            );
        }

        Room room = CreateRuntimeObject<Room>();
        room.name = roomName;
        room.Name = roomName;
        room.IndividualRoomTemplates.Add(roomTemplate);

        runtimeGraph.Rooms.Add(room);
        return room;
    }

    private void ConnectRooms(LevelGraph runtimeGraph, RoomBase fromRoom, RoomBase toRoom)
    {
        Connection connection = CreateRuntimeObject<Connection>();
        connection.From = fromRoom;
        connection.To = toRoom;

        runtimeGraph.Connections.Add(connection);
    }

    private T CreateRuntimeObject<T>() where T : ScriptableObject
    {
        T createdObject = ScriptableObject.CreateInstance<T>();
        createdRuntimeObjects.Add(createdObject);
        return createdObject;
    }

    private int GetRandomInclusive(System.Random random, int minimum, int maximum)
    {
        return random.Next(minimum, maximum + 1);
    }

    private void Shuffle<T>(List<T> values, System.Random random)
    {
        for (int index = values.Count - 1; index > 0; index--)
        {
            int swapIndex = random.Next(index + 1);
            (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
        }
    }
}
