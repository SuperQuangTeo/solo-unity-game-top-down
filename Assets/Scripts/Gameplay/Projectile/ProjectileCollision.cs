using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] private Projectile projectile;

    [SerializeField] private LayerMask collisionLayers;

    [SerializeField] private int damageAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        bool isValidCollision = (collisionLayers.value & (1 << other.gameObject.layer)) != 0;

        if (!isValidCollision)
        {
            return;
        }

        if (other.TryGetComponent<EnemyHealth>(out EnemyHealth enemyHealth))
        {
            enemyHealth.TakeDamage(damageAmount);
        }

        projectile.ReturnToPool();
    }
}
