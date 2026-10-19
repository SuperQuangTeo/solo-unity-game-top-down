using System.Collections.Generic;
using UnityEngine;

public class MinimapGenerator : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform roomIconContainer;
    [SerializeField] private MinimapRoomIcon roomIconPrefab;
    [SerializeField] private RectTransform connectionContainer;
    [SerializeField] private RectTransform connectionPrefab;

    [Header("Minimap Layout")]
    [SerializeField, Min(0.1f)] private float roomIconSpacing = 70f;

    private readonly List<MinimapRoomIcon> roomIcons = new List<MinimapRoomIcon>();

    private readonly Dictionary<int, int> minimapXCoordinates = new Dictionary<int, int>();
    private readonly Dictionary<int, int> minimapYCoordinates = new Dictionary<int, int>();

    private RoomContext currentRoom;
    private Vector2Int minimapOrigin;

    public void BuildMinimap(RoomContext[] roomContexts)
    {
        if (!ValidateReferences(roomContexts))
        {
            return;
        }
        ClearMinimap();

        RoomContext startRoom = FindStartRoom(roomContexts);

        if (startRoom == null)
        {
            return;
        }

        minimapOrigin = startRoom.LayoutPosition;

        BuildFixedCoordinateMaps(roomContexts);

        foreach (RoomContext roomContext in roomContexts)
        {
            if (roomContext == null)
            {
                continue;
            }
            CreateRoomIcon(roomContext, minimapOrigin);
        }

        CreateRoomConnections(roomContexts, minimapOrigin);

        SetCurrentRoom(startRoom);
    }

    private bool ValidateReferences(RoomContext[] roomContexts)
    {
        if (roomIconContainer == null)
        {
            return false;
        }

        if (roomIconPrefab == null)
        {
            return false;
        }

        if (connectionContainer == null)
        {
            return false;
        }

        if (connectionPrefab == null)
        {
            return false;
        }

        if (roomContexts == null || roomContexts.Length == 0)
        {
            return false;
        }

        return true;
    }

    private RoomContext FindStartRoom(RoomContext[] roomContexts)
    {
        foreach (RoomContext roomContext in roomContexts)
        {
            if (roomContext == null)
            {
                continue;
            }

            if (roomContext.RoomType == RoomType.Start)
            {
                return roomContext;
            }
        }

        return null;
    }

    private void CreateRoomIcon(RoomContext roomContext, Vector2Int minimapOrigin)
    {
        MinimapRoomIcon createdRoomIcon = Instantiate(roomIconPrefab, roomIconContainer);

        createdRoomIcon.Initialize(roomContext);

        roomIcons.Add(createdRoomIcon);

        RectTransform createdRoomIconRectTransform = createdRoomIcon.GetComponent<RectTransform>();

        Vector2 minimapPosition = CalculateMinimapPosition(roomContext, minimapOrigin);

        createdRoomIconRectTransform.anchoredPosition = minimapPosition;

        createdRoomIcon.name = $"RoomIcon_{roomContext.RoomId}_{roomContext.RoomType}";
    }

    private void CreateRoomConnections(RoomContext[] roomContexts, Vector2Int minimapOrigin)
    {
        foreach (RoomContext roomContext in roomContexts)
        {
            if (roomContext == null)
            {
                continue;
            }

            if (roomContext.ConnectedRooms == null)
            {
                continue;
            }

            foreach (RoomContext connectedRoomContext in roomContext.ConnectedRooms)
            {
                if (connectedRoomContext == null)
                {
                    continue;
                }

                if (roomContext.RoomId >= connectedRoomContext.RoomId)
                {
                    continue;
                }

                CreateConnection(roomContext, connectedRoomContext, minimapOrigin);
            }
        }
    }

    private void CreateConnection(RoomContext firstRoom, RoomContext secondRoom, Vector2Int minimapOrigin)
    {
        Vector2 firstRoomPosition = CalculateMinimapPosition(firstRoom, minimapOrigin);

        Vector2 secondRoomPosition = CalculateMinimapPosition(secondRoom, minimapOrigin);

        Vector2 connectionDirection = secondRoomPosition - firstRoomPosition;

        float connectionLength = connectionDirection.magnitude;

        Vector2 connectionMidPoint = (firstRoomPosition + secondRoomPosition) / 2f;

        float connectionAngle = Mathf.Atan2(connectionDirection.y, connectionDirection.x) * Mathf.Rad2Deg;

        RectTransform createdConnection = Instantiate(connectionPrefab, connectionContainer);

        createdConnection.anchoredPosition = connectionMidPoint;

        createdConnection.sizeDelta = new Vector2(connectionLength, createdConnection.sizeDelta.y);

        createdConnection.localRotation = Quaternion.Euler(0f, 0f, connectionAngle);

        createdConnection.name = $"Connection_{firstRoom.RoomId}_{secondRoom.RoomId}";
    }

    private Vector2 CalculateMinimapPosition(RoomContext roomContext, Vector2Int minimapOrigin)
    {
        int roomX = minimapXCoordinates[roomContext.LayoutPosition.x];
        int roomY = minimapYCoordinates[roomContext.LayoutPosition.y];

        int originX = minimapXCoordinates[minimapOrigin.x];
        int originY = minimapYCoordinates[minimapOrigin.y];

        int relativeX = roomX - originX;
        int relativeY = roomY - originY;

        return new Vector2(relativeX * roomIconSpacing, relativeY * roomIconSpacing);
    }

    private void BuildFixedCoordinateMaps(RoomContext[] roomContexts)
    {
        minimapXCoordinates.Clear();
        minimapYCoordinates.Clear();

        HashSet<int> uniqueXCoordinates = new HashSet<int>();
        HashSet<int> uniqueYCoordinates = new HashSet<int>();

        foreach (RoomContext roomContext in roomContexts)
        {
            if (roomContext == null)
            {
                continue;
            }

            uniqueXCoordinates.Add(roomContext.LayoutPosition.x);
            uniqueYCoordinates.Add(roomContext.LayoutPosition.y);
        }

        List<int> sortedXCoordinates = new List<int>(uniqueXCoordinates);

        List<int> sortedYCoordinates = new List<int>(uniqueYCoordinates);

        sortedXCoordinates.Sort();
        sortedYCoordinates.Sort();

        for (int index = 0; index < sortedXCoordinates.Count; index++)
        {
            minimapXCoordinates.Add(sortedXCoordinates[index], index);
        }

        for (int index = 0; index < sortedYCoordinates.Count; index++)
        {
            minimapYCoordinates.Add(sortedYCoordinates[index], index);
        }
    }

    private void ClearMinimap()
    {
        for (int childIndex = roomIconContainer.childCount - 1; childIndex >= 0; childIndex--)
        {
            Transform childTransform = roomIconContainer.GetChild(childIndex);

            Destroy(childTransform.gameObject);
        }

        for (int childIndex = connectionContainer.childCount - 1; childIndex >= 0; childIndex--)
        {
            Transform childTransform = connectionContainer.GetChild(childIndex);

            Destroy(
                childTransform.gameObject
            );
        }
        roomIcons.Clear();
        minimapXCoordinates.Clear();
        minimapYCoordinates.Clear();
        currentRoom = null;
    }

    public void SetCurrentRoom(RoomContext newCurrentRoom)
    {
        if (newCurrentRoom == null)
        {
            return;
        }

        currentRoom = newCurrentRoom;

        currentRoom.MarkAsVisited();

        RecenterMinimap();

        RefreshRoomIcons();
    }

    private void RecenterMinimap()
    {
        if (currentRoom == null)
        {
            return;
        }

        Vector2 currentRoomPosition = CalculateMinimapPosition(currentRoom, minimapOrigin);

        Vector2 minimapOffset = -currentRoomPosition;

        roomIconContainer.anchoredPosition = minimapOffset;
        connectionContainer.anchoredPosition = minimapOffset;
    }

    private void RefreshRoomIcons()
    {
        foreach (MinimapRoomIcon roomIcon in roomIcons)
        {
            if (roomIcon == null)
            {
                continue;
            }

            bool isCurrentRoom = roomIcon.RoomContext == currentRoom;
            roomIcon.Refresh(isCurrentRoom);
        }
    }
}
