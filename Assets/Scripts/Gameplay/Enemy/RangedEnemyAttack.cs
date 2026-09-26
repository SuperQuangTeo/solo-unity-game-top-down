using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RangedEnemyAttack : EnemyAttackBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Attack Range")]
    [SerializeField] private float attackRange = 5f;

    [Header("Projectile")]
    [SerializeField] private ProjectilePool projectilePool;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float projectileSpawnOffset = 0.5f;

    [Header("Attack Visual")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float attackTelegraphDuration = 0.25f;

    [SerializeField]
    private Vector3 attackPunchScale =
        new Vector3(0.15f, 0.15f, 0f);

    private bool isAttacking;

    private float nextAttackTime;

    private Vector3 originalVisualScale;

    private Coroutine attackCoroutine;

    private void Awake()
    {
        if (visualTransform != null)
        {
            originalVisualScale = visualTransform.localScale;
        }
    }

    public void SetProjectilePool(ProjectilePool newProjectilePool)
    {
        projectilePool = newProjectilePool;
    }

    public override bool CanAttack(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position,playerTransform.position);

        return distanceToPlayer <= attackRange;
    }

    public override void UpdateAttack(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return;
        }

        if (enemyData == null)
        {
            return;
        }

        if (projectilePool == null)
        {
            return;
        }

        if (firePoint == null)
        {
            return;
        }

        if (isAttacking)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        attackCoroutine = StartCoroutine(AttackCoroutine(playerTransform));
    }

    private IEnumerator AttackCoroutine(Transform playerTransform)
    {
        isAttacking = true;

        PlayAttackTelegraph();

        yield return new WaitForSeconds(attackTelegraphDuration);

        if (playerTransform != null)
        {
            ShootProjectile(playerTransform);
        }

        nextAttackTime = Time.time + enemyData.AttackCooldown;

        isAttacking = false;

        attackCoroutine = null;
    }

    private void ShootProjectile(Transform playerTransform)
    {
        Vector2 shootDirection = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

        Projectile projectile = projectilePool.GetProjectile();

        Vector2 spawnPosition = (Vector2)firePoint.position + shootDirection * projectileSpawnOffset;

        projectile.transform.position = spawnPosition;

        projectile.Initialize(
            shootDirection,
            projectileSpeed,
            projectileLifetime
        );
    }

    private void PlayAttackTelegraph()
    {
        if (visualTransform == null)
        {
            return;
        }

        visualTransform.DOKill();

        visualTransform.localScale = originalVisualScale;

        visualTransform.DOPunchScale(
            attackPunchScale,
            attackTelegraphDuration,
            5,
            0.5f
        );
    }

    public override void ResetAttack()
    {
        StopCurrentAttack();
        nextAttackTime = 0f;
        ResetAttackVisual();
    }

    private void StopCurrentAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
    }

    private void ResetAttackVisual()
    {
        if (visualTransform == null)
        {
            return;
        }

        visualTransform.DOKill();

        visualTransform.localScale = originalVisualScale;
    }

    private void OnDisable()
    {
        ResetAttack();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}