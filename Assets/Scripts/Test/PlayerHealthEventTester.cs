using UnityEngine;

public class PlayerHealthEventTester : MonoBehaviour
{
    [Header("References")]

    [SerializeField] private PlayerHealth playerHealth;


    private void OnEnable()
    {
        playerHealth.OnHealthChanged += HandleHealthChanged;

        playerHealth.OnDeath += HandleDeath;
    }


    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= HandleHealthChanged;

        playerHealth.OnDeath -= HandleDeath;
    }


    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Debug.Log(
            $"[EVENT TEST] Player Health Changed: {currentHealth}/{maxHealth}",
            this
        );
    }


    private void HandleDeath()
    {
        Debug.Log(
            "[EVENT TEST] Player Death Event ran.",
            this
        );
    }
}
