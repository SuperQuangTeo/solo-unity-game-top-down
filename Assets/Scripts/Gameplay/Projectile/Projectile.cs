using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D projectileRigidbody;

    private Vector2 moveDirection;

    private float moveSpeed;

    private bool isActive;

    private float remainingLifetime;

    private ProjectilePool ownerPool;

    public void Initialize(Vector2 direction, float speed)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        isActive = true;
    }


    public void SetOwnerPool(ProjectilePool pool)
    {
        ownerPool = pool;
    }


    public void Initialize(Vector2 direction, float speed, float lifetime)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        remainingLifetime = lifetime;
        isActive = true;
    }


    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime > 0f)
        {
            return;
        }
        ReturnToPool();
    }


    private void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }

        Vector2 newPosition = projectileRigidbody.position
            + moveDirection * moveSpeed * Time.fixedDeltaTime;

        projectileRigidbody.MovePosition(newPosition);
    }


    public void ReturnToPool()
    {
        if (!isActive)
        {
            return;
        }

        isActive = false;

        ownerPool.ReleaseProjectile(this);
    }


    public void Stop()
    {
        isActive = false;

        moveDirection = Vector2.zero;
        moveSpeed = 0f;
        remainingLifetime = 0f;
        projectileRigidbody.linearVelocity = Vector2.zero;
    }
}
