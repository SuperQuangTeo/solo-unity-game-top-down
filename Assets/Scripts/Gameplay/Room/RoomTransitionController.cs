using UnityEngine;

public class RoomTransitionController : MonoBehaviour
{
    [Header("Room Transition References")]
    [SerializeField] private CameraController cameraController;

    [SerializeField] private MinimapGenerator minimapGenerator;

    [Header("Runtime State")]
    [SerializeField] private RoomContext currentRoom;

    [Header("Player Reference")]
    [SerializeField] private Rigidbody2D playerRigidbody;


    public RoomContext CurrentRoom => currentRoom;

    public void SetInitialRoom(RoomContext initialRoom)
    {
        if (initialRoom == null)
        {
            return;
        }

        currentRoom = initialRoom;

        PlacePlayerAtRoomSpawnPoint(initialRoom);

        if (cameraController != null)
        {
            cameraController.FocusRoom(initialRoom);
        }
    }

    public void EnterRoom(RoomContext newRoom)
    {
        if (newRoom == null)
        {
            return;
        }
        if (newRoom == currentRoom)
        {
            return;
        }

        currentRoom = newRoom;

        if (cameraController != null)
        {
            cameraController.FocusRoom(newRoom);
        }

        if (minimapGenerator != null)
        {
            minimapGenerator.SetCurrentRoom(newRoom);
        }
    }

    private void PlacePlayerAtRoomSpawnPoint(RoomContext roomContext)
    {
        if (playerRigidbody == null)
        {
            return;
        }

        PlayerSpawnPoint playerSpawnPoint = roomContext.PlayerSpawnPoint;

        if (playerSpawnPoint == null)
        {
            return;
        }
        Vector2 spawnPosition = playerSpawnPoint.transform.position;
        playerRigidbody.position = spawnPosition;
    }
}