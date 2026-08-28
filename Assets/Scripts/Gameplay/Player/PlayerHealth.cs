using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerData playerData;

    private int currentHealth;

    private bool isInvincible;

    private bool isDead;

    public event Action<int, int> OnHealthChanged;

    public event Action OnDeath;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => playerData.MaxHealth;

    private void Awake()
    {
        InitializeHealth();
    }

    private void Start()
    {
        NotifyHealthChanged();
    }


    private void InitializeHealth()
    {
        if (playerData == null)
        {
            return;
        }
        currentHealth = playerData.MaxHealth;
        isDead = false;
        isInvincible = false;
    }


    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        if (damageAmount <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        Debug.Log(
            $"Player take {damageAmount} damage. HP left: {currentHealth}",
            this
        );

        NotifyHealthChanged();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityCoroutine());
    }


    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;

        yield return new WaitForSeconds(playerData.InvincibilityDuration);

        isInvincible = false;
    }


    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth, playerData.MaxHealth);
    }


    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        OnDeath?.Invoke();
    }
}