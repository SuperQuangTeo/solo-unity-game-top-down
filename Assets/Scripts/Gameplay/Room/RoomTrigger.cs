using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [Header("Room Reference")]
    [SerializeField] private RoomController roomController;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            return;
        }

        if (roomController == null)
        {
            return;
        }
        roomController.StartCombat();
    }
}