using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Transform firePoint;

    [SerializeField] private PlayerData playerData;

    [SerializeField] private ProjectilePool projectilePool;

    private float nextFireTime;

    private Vector2 shootDirection;

    public void SetShootDirection(Vector2 newShootDirection)
    {
        shootDirection = newShootDirection;

        TryShoot();
    }

    private void TryShoot()
    {
        if (shootDirection == Vector2.zero)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Shoot();

        nextFireTime = Time.time + playerData.FireInterval;
    }


    private void Shoot()
    {
        Projectile projectile = projectilePool.GetProjectile();

        Vector2 normalizedDirection = shootDirection.normalized;

        Vector2 spawnPosition = (Vector2)firePoint.position
            + normalizedDirection * playerData.ProjectileSpawnOffset;

        projectile.transform.position = spawnPosition;

        projectile.Initialize(normalizedDirection, playerData.ProjectileSpeed, playerData.ProjectileLifetime);
    }
}

