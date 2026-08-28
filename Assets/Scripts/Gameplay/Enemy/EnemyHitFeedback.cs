using DG.Tweening;
using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private SpriteRenderer visualSpriteRenderer;


    [Header("Hit Flash")]
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.08f;

    private Color originalColor;

    private void Awake()
    {
        originalColor = visualSpriteRenderer.color;
    }

    private void OnEnable()
    {
        enemyHealth.OnDamaged += PlayHitFlash;
    }

    private void OnDisable()
    {
        enemyHealth.OnDamaged -= PlayHitFlash;

        if (visualSpriteRenderer != null)
        {
            visualSpriteRenderer.DOKill();
            visualSpriteRenderer.color = originalColor;
        }
    }

    private void PlayHitFlash()
    {
        visualSpriteRenderer.DOKill();
        visualSpriteRenderer.color = originalColor;
        visualSpriteRenderer.DOColor(hitColor, hitFlashDuration).SetLoops(2,LoopType.Yoyo);
    }
}
