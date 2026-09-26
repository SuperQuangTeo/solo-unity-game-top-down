using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeleeEnemyAttack : EnemyAttackBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Attack Visual")]
    [SerializeField] private Transform visualTransform;

    [SerializeField] private float attackTelegraphDuration = 0.15f;

    [SerializeField]
    private Vector3 attackPunchScale =
        new Vector3(0.15f, 0.15f, 0f);

    private PlayerHealth touchingPlayerHealth;

    private bool isTouchingPlayer;

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

    public override bool CanAttack(Transform playerTransform)
    {
        return isTouchingPlayer && touchingPlayerHealth != null;
    }

    public override void UpdateAttack(Transform playerTransform)
    {
        if (enemyData == null)
        {
            return;
        }

        if (touchingPlayerHealth == null)
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

        attackCoroutine = StartCoroutine(
            AttackCoroutine(touchingPlayerHealth)
        );
    }

    private IEnumerator AttackCoroutine(PlayerHealth playerHealth)
    {
        isAttacking = true;

        PlayAttackTelegraph();

        yield return new WaitForSeconds(attackTelegraphDuration);

        if (isTouchingPlayer && playerHealth != null)
        {
            playerHealth.TakeDamage(enemyData.AttackDamage);
        }

        nextAttackTime =
            Time.time + enemyData.AttackCooldown;

        isAttacking = false;

        attackCoroutine = null;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }

        isTouchingPlayer = true;

        touchingPlayerHealth = playerHealth;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(
                out PlayerHealth playerHealth))
        {
            return;
        }

        isTouchingPlayer = false;

        touchingPlayerHealth = null;
    }

    public override void ResetAttack()
    {
        isTouchingPlayer = false;
        touchingPlayerHealth = null;

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
}