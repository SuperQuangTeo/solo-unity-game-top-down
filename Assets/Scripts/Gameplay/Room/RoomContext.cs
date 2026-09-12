using UnityEngine;

public class RoomContext : MonoBehaviour
{
    [Header("Room Identity")]
    [SerializeField] private RoomType roomType;
    [SerializeField] private int roomId = -1;

    [SerializeField] private Vector2Int layoutPosition;

    [Header("Runtime Spawn Points")]
    [SerializeField] private EnemySpawnPoint[] enemySpawnPoints;
    [SerializeField] private RewardSpawnPoint rewardSpawnPoint;
    [SerializeField] private PlayerSpawnPoint playerSpawnPoint;

    [Header("Room State")]
    [SerializeField] private bool isVisited;

    [Header("Room Connections")]
    [SerializeField] private RoomContext[] connectedRooms;

    [Header("Camera")]
    [SerializeField] private Transform cameraAnchor;

    public int RoomId => roomId;
    public RoomType RoomType => roomType;
    public EnemySpawnPoint[] EnemySpawnPoints => enemySpawnPoints;
    public RewardSpawnPoint RewardSpawnPoint => rewardSpawnPoint;
    public PlayerSpawnPoint PlayerSpawnPoint => playerSpawnPoint;
    public bool IsVisited => isVisited;
    public RoomContext[] ConnectedRooms => connectedRooms;
    public Vector2Int LayoutPosition => layoutPosition;
    public Transform CameraAnchor => cameraAnchor;

    public void SetRoomId(int newRoomId)
    {
        roomId = newRoomId;
    }

    public void SetEnemySpawnPoints(EnemySpawnPoint[] newEnemySpawnPoints)
    {
        enemySpawnPoints = newEnemySpawnPoints;
    }

    public void SetRewardSpawnPoint(RewardSpawnPoint newRewardSpawnPoint)
    {
        rewardSpawnPoint = newRewardSpawnPoint;
    }

    public void SetLayoutPosition(Vector2Int newLayoutPosition)
    {
        layoutPosition = newLayoutPosition;
    }

    public void SetConnectedRooms(RoomContext[] newConnectedRooms)
    {
        connectedRooms = newConnectedRooms;
    }

    public void SetPlayerSpawnPoint(PlayerSpawnPoint newPlayerSpawnPoint)
    {
        playerSpawnPoint = newPlayerSpawnPoint;
    }

    public void MarkAsVisited()
    {
        if (isVisited)
        {
            return;
        }
        isVisited = true;
    }
}