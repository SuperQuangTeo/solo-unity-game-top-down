using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Player Detection")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack Visual")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private float attackTelegraphDuration = 0.15f;
    [SerializeField] private Vector3 attackPunchScale = new Vector3(0.15f, 0.15f, 0f);

    private Rigidbody2D enemyRigidbody;

    private EnemyHealth enemyHealth;

    private bool isPlayerDetected;

    private bool isTouchingPlayer;

    private bool isAttacking;

    private bool isDead;

    private float nextAttackTime;

    private Vector3 originalVisualScale;

    private Coroutine attackCoroutine;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (visualTransform != null)
        {
            originalVisualScale = visualTransform.localScale;
        }
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += HandleDeath;
        }
    }


    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= HandleDeath;
        }
    }

    private void Update()
    {
        DetectPlayer();
    }

    private void FixedUpdate()
    {
        if (isPlayerDetected && !isTouchingPlayer)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (isDead) return;
        if (playerTransform == null || enemyData == null)
        {
            return;
        }
        Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

        Vector2 nextPosition = enemyRigidbody.position + directionToPlayer * enemyData.MoveSpeed * Time.fixedDeltaTime;

        enemyRigidbody.MovePosition(nextPosition);
    }

    private void DetectPlayer()
    {
        if (playerTransform == null)
        {
            isPlayerDetected = false;
            return;
        }
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        isPlayerDetected = distanceToPlayer <= detectionRange;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }
        isTouchingPlayer = true;

        TryAttack(playerHealth);
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }
        isTouchingPlayer = false;
    }


    private void TryAttack(PlayerHealth playerHealth)
    {
        if (isDead)
        {
            return;
        }
        if (enemyData == null)
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
        attackCoroutine =  StartCoroutine(AttackCoroutine(playerHealth));
    }


    private IEnumerator AttackCoroutine(PlayerHealth playerHealth)
    {
        isAttacking = true;

        PlayAttackTelegraph();

        yield return new WaitForSeconds(attackTelegraphDuration);

        if (!isDead && isTouchingPlayer && playerHealth != null)
        {
            playerHealth.TakeDamage(enemyData.AttackDamage);
        }
        nextAttackTime = Time.time + enemyData.AttackCooldown;
        isAttacking = false;
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

    private void HandleDeath()
    {
        isDead = true;
        isPlayerDetected = false;
        isTouchingPlayer = false;
        isAttacking = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        if (visualTransform != null)
        {
            visualTransform.DOKill();
            visualTransform.localScale = originalVisualScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
