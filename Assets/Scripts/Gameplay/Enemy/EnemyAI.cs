using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Data")]
    [SerializeField] private EnemyData enemyData;

    [Header("Player Detection")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float detectionRange = 5f;

    [Header("Attack Behaviour")]
    [SerializeField] private EnemyAttackBehaviour attackBehaviour;

    private Rigidbody2D enemyRigidbody;

    private EnemyHealth enemyHealth;

    private bool isPlayerDetected;

    private bool isDead;

    private EnemyState currentState = EnemyState.Idle;

    private void Awake()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
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

        if (attackBehaviour != null)
        {
            attackBehaviour.ResetAttack();
        }

        ResetRigidbody();
    }

    private void Update()
    {
        DetectPlayer();

        if (isDead)
        {
            return;
        }

        UpdateStateFromPlayerConditions();

        UpdateCurrentState();
    }

    private void UpdateStateFromPlayerConditions()
    {
        if (isPlayerDetected && attackBehaviour != null && attackBehaviour.CanAttack(playerTransform))
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        if (isPlayerDetected)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        ChangeState(EnemyState.Idle);
    }

    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                break;

            case EnemyState.Chase:
                break;

            case EnemyState.Attack:
                if (attackBehaviour != null)
                {
                    attackBehaviour.UpdateAttack(playerTransform);
                }
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        if (currentState != EnemyState.Chase)
        {
            return;
        }

        UpdateChaseState();
    }

    private void UpdateChaseState()
    {
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

    public void ResetForReuse()
    {
        isDead = false;

        currentState = EnemyState.Idle;

        isPlayerDetected = false;

        if (attackBehaviour != null)
        {
            attackBehaviour.ResetAttack();
        }

        ResetRigidbody();
    }

    private void ResetRigidbody()
    {
        if (enemyRigidbody == null)
        {
            return;
        }
        enemyRigidbody.linearVelocity = Vector2.zero;
        enemyRigidbody.angularVelocity = 0f;
    }

    private void HandleDeath(EnemyHealth deadEnemyHealth)
    {
        isDead = true;

        isPlayerDetected = false;

        if (attackBehaviour != null)
        {
            attackBehaviour.ResetAttack();
        }

        ResetRigidbody();
    }

    public void SetPlayerTransform(Transform newPlayerTransform)
    {
        playerTransform = newPlayerTransform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
