using UnityEngine;
using UnityEngine.UI;

public class MinimapRoomIcon : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image roomImage;

    [Header("Room State Colors")]
    [SerializeField] private Color unvisitedColor = Color.gray;
    [SerializeField] private Color visitedColor = Color.white;
    [SerializeField] private Color currentColor = Color.yellow;

    private RoomContext roomContext;

    public RoomContext RoomContext => roomContext;

    public void Initialize(RoomContext newRoomContext)
    {
        if (newRoomContext == null)
        {
            return;
        }
        roomContext = newRoomContext;

        Refresh(false);
    }


    public void Refresh(bool isCurrentRoom)
    {
        if (roomImage == null)
        {
            return;
        }

        if (roomContext == null)
        {
            return;
        }

        if (isCurrentRoom)
        {
            roomImage.color = currentColor;
            return;
        }

        if (roomContext.IsVisited)
        {
            roomImage.color = visitedColor;
            return;
        }

        roomImage.color = unvisitedColor;
    }
}