using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : MonoBehaviour
{
    [Header("Heart UI")]
    [SerializeField] private Transform heartContainer;
    [SerializeField] private Image heartPrefab;

    [Header("Heart Sprites")]
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite halfHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Heart Animation")]
    [SerializeField] private float heartPunchScale = 0.15f;
    [SerializeField] private float heartPunchDuration = 0.2f;

    [Header("Currency UI")]
    [SerializeField] private TMP_Text coinText;

    [Header("Currency Animation")]
    [SerializeField] private float currencyPunchScale = 0.15f;
    [SerializeField] private float currencyPunchDuration = 0.2f;

    private Vector3 originalCoinTextScale;
    private bool hasInitializedCurrency;
    private int displayedCurrency;

    private bool hasInitializedHealth;

    private readonly List<Image> heartImages = new List<Image>();
    private readonly List<Vector3> originalHeartScales = new List<Vector3>();

    private void Awake()
    {
        originalCoinTextScale = coinText.rectTransform.localScale;
    }


    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        int requiredHeartCount = Mathf.CeilToInt(maxHealth / 2f);

        EnsureHeartCount(requiredHeartCount);

        for (int heartIndex = 0; heartIndex < heartImages.Count; heartIndex++)
        {
            if (heartIndex >= requiredHeartCount)
            {
                heartImages[heartIndex].gameObject.SetActive(false);
                continue;
            }

            heartImages[heartIndex].gameObject.SetActive(true);

            int healthForThisHeart = currentHealth - (heartIndex * 2);

            Sprite previousHeartSprite = heartImages[heartIndex].sprite;

            UpdateHeartSprite(heartImages[heartIndex], healthForThisHeart);

            bool heartStateChanged = previousHeartSprite != heartImages[heartIndex].sprite;

            if (hasInitializedHealth && heartStateChanged)
            {
                PlayHeartPunchAnimation(heartIndex);
            }
            hasInitializedHealth = true;

        }
    }

    private void EnsureHeartCount(int requiredHeartCount)
    {
        while (heartImages.Count < requiredHeartCount)
        {
            CreateHeart();
        }
    }

    private void CreateHeart()
    {
        Image newHeart = Instantiate(heartPrefab, heartContainer);

        heartImages.Add(newHeart);

        originalHeartScales.Add(
            newHeart.rectTransform.localScale
        );
    }

    private void UpdateHeartSprite(Image heartImage, int healthForThisHeart)
    {
        if (healthForThisHeart >= 2)
        {
            heartImage.sprite = fullHeartSprite;
            return;
        }

        if (healthForThisHeart == 1)
        {
            heartImage.sprite = halfHeartSprite;
            return;
        }

        heartImage.sprite = emptyHeartSprite;
    }

    private void PlayHeartPunchAnimation(int heartIndex)
    {
        RectTransform heartRectTransform = heartImages[heartIndex].rectTransform;

        heartRectTransform.DOKill();

        heartRectTransform.localScale = originalHeartScales[heartIndex];

        heartRectTransform.DOPunchScale(Vector3.one * heartPunchScale, heartPunchDuration, 5, 0.5f);
    }

    public void UpdateCurrency(int currencyAmount)
    {
        bool currencyChanged = hasInitializedCurrency && displayedCurrency != currencyAmount;

        coinText.text = currencyAmount.ToString();

        displayedCurrency = currencyAmount;

        if (currencyChanged)
        {
            PlayCurrencyPunchAnimation();
        }

        hasInitializedCurrency = true;
    }

    private void PlayCurrencyPunchAnimation()
    {
        RectTransform coinTextRectTransform = coinText.rectTransform;

        coinTextRectTransform.DOKill();

        coinTextRectTransform.localScale = originalCoinTextScale;

        coinTextRectTransform.DOPunchScale(Vector3.one * currencyPunchScale, currencyPunchDuration, 5, 0.5f);
    }
}
