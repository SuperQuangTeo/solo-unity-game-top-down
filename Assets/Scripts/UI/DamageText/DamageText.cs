using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text damageText;

    [Header("Animation")]
    [SerializeField] private float moveDistance = 0.5f;
    [SerializeField] private float animationDuration = 0.5f;

    private DamageTextPool ownerPool;

    private Vector3 originalScale;

    private Color originalColor;

    private Sequence animationSequence;


    private void Awake()
    {
        if (damageText != null)
        {
            originalColor = damageText.color;
        }

        originalScale = transform.localScale;
    }

    public void SetOwnerPool(DamageTextPool newOwnerPool)
    {
        ownerPool = newOwnerPool;
    }


    public void ShowDamage(int damageAmount)
    {
        if (damageText == null)
        {
            return;
        }

        damageText.text = damageAmount.ToString();

        PlayAnimation();
    }

    public void ResetForReuse()
    {
        KillAnimation();

        transform.localScale = originalScale;

        if (damageText != null)
        {
            damageText.color = originalColor;
        }
    }

    private void PlayAnimation()
    {
        KillAnimation();

        Vector3 targetPosition =
            transform.position + Vector3.up * moveDistance;

        animationSequence = DOTween.Sequence();

        animationSequence.Join(
            transform.DOMove(
                targetPosition,
                animationDuration
            )
        );

        animationSequence.Join(
            damageText.DOFade(
                0f,
                animationDuration
            )
        );

        animationSequence.OnComplete(
            ReturnToPool
        );
    }

    private void KillAnimation()
    {
        if (animationSequence != null)
        {
            animationSequence.Kill();

            animationSequence = null;
        }

        if (damageText != null)
        {
            damageText.DOKill();
        }

        transform.DOKill();
    }

    private void ReturnToPool()
    {
        if (ownerPool == null)
        {
            return;
        }

        ownerPool.ReleaseDamageText(this);
    }

    private void OnDisable()
    {
        KillAnimation();
    }
}