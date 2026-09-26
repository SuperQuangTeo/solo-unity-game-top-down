using UnityEngine;

public class EnemyProjectileCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Projectile projectile;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayers;

    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isValidCollision = (collisionLayers.value & (1 << other.gameObject.layer)) != 0;

        if (!isValidCollision)
        {
            return;
        }

        if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damageAmount);
        }

        projectile.ReturnToPool();
    }
}