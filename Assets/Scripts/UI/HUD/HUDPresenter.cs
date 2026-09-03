using UnityEngine;

public class HUDPresenter : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerResources playerResources;

    [Header("View")]
    [SerializeField] private HUDView hudView;


    private void OnEnable()
    {
        playerHealth.OnHealthChanged += HandleHealthChanged;
        playerResources.OnCoinsChanged += HandleCurrencyChanged;
    }

    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= HandleHealthChanged;
        playerResources.OnCoinsChanged -= HandleCurrencyChanged;
    }

    private void Start()
    {
        RefreshHealth();
        RefreshCurrency();
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        hudView.UpdateHealth(currentHealth, maxHealth);
    }

    private void HandleCurrencyChanged(int currentCurrency)
    {
        hudView.UpdateCurrency(currentCurrency);
    }

    private void RefreshHealth()
    {
        hudView.UpdateHealth(
            playerHealth.CurrentHealth,
            playerHealth.MaxHealth
        );
    }

    private void RefreshCurrency()
    {
        hudView.UpdateCurrency(playerResources.CurrentCoins);
    }
}