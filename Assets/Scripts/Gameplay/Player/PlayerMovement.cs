using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private PlayerData playerData;

    private Vector2 moveDirection;

    private bool isDashing;
    private Vector2 dashDirection;
    private float dashSpeed;

    public Vector2 CurrentMoveDirection => moveDirection;

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }
    public void StartDash(Vector2 direction, float speed)
    {
        isDashing = true;
        dashDirection = direction.normalized;
        dashSpeed = speed;
    }
    public void StopDash()
    {
        isDashing = false;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            MoveDuringDash();
            return;
        }
        MoveNormally();
    }

    private void MoveNormally()
    {
        Vector2 newPosition = playerRigidbody.position
            + moveDirection * playerData.MoveSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(newPosition);
    }


    private void MoveDuringDash()
    {
        Vector2 newPosition = playerRigidbody.position
            + dashDirection * dashSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(newPosition);
    }

}
