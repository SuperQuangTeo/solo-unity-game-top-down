using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image progressFill;
    [SerializeField] private CanvasGroup canvasGroup;

    public void Show(Action onComplete = null)
    {
        gameObject.SetActive(true);
        canvasGroup.DOKill();

        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, 0.3f).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void Hide(Action onComplete = null)
    {
        canvasGroup.DOKill();

        canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void SetProgress(float progress)
    {
        progressFill.fillAmount = progress;
    }
}
