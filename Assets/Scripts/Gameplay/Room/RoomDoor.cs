using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    [Header("Door References")]
    [SerializeField] private Collider2D doorCollider;

    [Header("Door State")]
    [SerializeField] private bool isLocked;

    public bool IsLocked => isLocked;


    private void Awake()
    {
        UnlockDoor();
    }


    public void LockDoor()
    {
        if (isLocked)
        {
            return;
        }
        isLocked = true;

        if (doorCollider != null)
        {
            doorCollider.enabled = true;
        }
    }


    public void UnlockDoor()
    {
        isLocked = false;

        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }
    }
}