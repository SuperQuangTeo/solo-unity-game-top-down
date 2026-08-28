using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction;

    [SerializeField] private InputActionReference shoootAction;

    [SerializeField] private InputActionReference dashAction;

    [SerializeField] private PlayerMovement playerMovement;

    [SerializeField] private PlayerDash playerDash;

    [SerializeField] private PlayerShooter playerShooter;

    private void OnEnable()
    {
        moveAction.action.Enable();
        dashAction.action.Enable();
        shoootAction.action.Enable();

        dashAction.action.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        dashAction.action.Disable();
        shoootAction.action.Disable();

        dashAction.action.performed -= OnDashPerformed;
    }

    private void Update()
    {
        Vector2 moveDirection = moveAction.action.ReadValue<Vector2>();
        Vector2 shootDirection = shoootAction.action.ReadValue<Vector2>();

        playerMovement.SetMoveDirection(moveDirection);
        playerShooter.SetShootDirection(shootDirection);
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        playerDash.TryDash();
    }
}
