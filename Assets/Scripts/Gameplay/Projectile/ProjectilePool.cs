using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;

    [SerializeField] private int defaultCapacity = 10;

    [SerializeField] private int maxSize = 30;

    private ObjectPool<Projectile> projectilePool;


    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(
            CreateProjectile,
            OnGetProjectile,
            OnReleaseProjectile,
            OnDestroyProjectile,
            true,
            defaultCapacity,
            maxSize
        );
    }


    private Projectile CreateProjectile()
    {
        Projectile projectile = Instantiate(
            projectilePrefab,
            transform
        );

        projectile.SetOwnerPool(this);

        projectile.gameObject.SetActive(false);

        return projectile;
    }


    private void OnGetProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(true);
    }


    private void OnReleaseProjectile(Projectile projectile)
    {
        projectile.Stop();
        projectile.gameObject.SetActive(false);
    }


    private void OnDestroyProjectile(Projectile projectile)
    {
        if (projectile == null)
        {
            return;
        }
        Destroy(projectile.gameObject);
    }


    public Projectile GetProjectile()
    {
        return projectilePool.Get();
    }


    public void ReleaseProjectile(Projectile projectile)
    {
        projectilePool.Release(projectile);
    }
}
