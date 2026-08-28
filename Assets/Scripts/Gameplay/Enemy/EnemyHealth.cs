using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Data")]

    [SerializeField] private EnemyData enemyData;

    private int currentHealth;

    private bool isDead;

    public event Action OnDeath;
    public event Action OnDamaged;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        InitializeHealth();
    }

    private void InitializeHealth()
    {
        if (enemyData == null)
        {
            return;
        }
        currentHealth = enemyData.MaxHealth;

        isDead = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || damageAmount <= 0)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        OnDamaged?.Invoke();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDeath?.Invoke();
    }
}
