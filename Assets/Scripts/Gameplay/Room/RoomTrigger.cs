using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Room Reference")]
    [SerializeField] private RoomController roomController;

    private RoomContext roomContext;

    private RoomTransitionController roomTransitionController;

    private void Awake()
    {
        if (roomController != null)
        {
            roomContext = roomController.GetComponent<RoomContext>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            return;
        }

        if (roomTransitionController != null && roomContext != null)
        {
            roomTransitionController.EnterRoom(roomContext);
        }

        if (roomController != null)
        {
            roomController.StartCombat();
        }
    }


    public void SetRoomTransitionController(RoomTransitionController newRoomTransitionController)
    {
        roomTransitionController = newRoomTransitionController;
    }
}