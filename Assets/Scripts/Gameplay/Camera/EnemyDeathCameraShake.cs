using UnityEngine;

public class EnemyDeathCameraShake : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private CameraController cameraController;

    [Header("Shake Settings")]
    [SerializeField] private float deathShakeForce = 0.7f;

    private void OnEnable()
    {
        if (enemyHealth == null)
        {
            return;
        }
        enemyHealth.OnDeath += HandleEnemyDeath;
    }

    private void OnDisable()
    {
        if (enemyHealth == null)
        {
            return;
        }
        enemyHealth.OnDeath -= HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        if (cameraController == null)
        {
            return;
        }
        cameraController.ShakeCamera(deathShakeForce);
    }
}