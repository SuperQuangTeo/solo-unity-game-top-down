using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Transform playerVisual;
    private bool canDash = true;

    public void TryDash()
    {
        if (!canDash)
        {
            return;
        }

        Vector2 dashDirection = playerMovement.CurrentMoveDirection;

        if (dashDirection == Vector2.zero)
        {
            return;
        }

        StartCoroutine(DashRoutine(dashDirection));
    }


    private IEnumerator DashRoutine(Vector2 dashDirection)
    {
        canDash = false;

        PlayDashPunchEffect();

        playerMovement.StartDash(dashDirection, playerData.DashSpeed);

        yield return new WaitForSeconds(playerData.DashDuration);

        playerMovement.StopDash();

        yield return new WaitForSeconds(playerData.DashCooldown);

        canDash = true;
    }

    private void PlayDashPunchEffect()
    {
        playerVisual.DOKill();

        playerVisual.DOPunchScale(
            new Vector3(0.15f, -0.1f, 0f),
            0.2f,
            6,
            0.5f
        );
    }
}
