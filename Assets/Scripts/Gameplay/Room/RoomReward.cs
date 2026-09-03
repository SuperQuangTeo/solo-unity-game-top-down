using UnityEngine;

public class RoomReward : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RoomController roomController;
    [SerializeField] private PlayerResources playerResources;

    [Header("Reward")]
    [SerializeField] private int coinRewardAmount = 1;

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
        if (roomController == null)
        {
            return;
        }
        roomController.OnRoomCleared -= HandleRoomCleared;
    }

    private void HandleRoomCleared()
    {
        if (playerResources == null)
        {
            return;
        }
        if (coinRewardAmount <= 0)
        {
            return;
        }
        playerResources.AddCoins(coinRewardAmount);
    }
}