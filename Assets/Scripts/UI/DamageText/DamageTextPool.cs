using UnityEngine;
using UnityEngine.Pool;

public class DamageTextPool : MonoBehaviour
{
    [Header("Damage Text")]
    [SerializeField] private DamageText damageTextPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 10;

    [SerializeField] private int maxSize = 30;

    private ObjectPool<DamageText> damageTextPool;

    private void Awake()
    {
        damageTextPool = new ObjectPool<DamageText>(
            CreateDamageText,
            OnGetDamageText,
            OnReleaseDamageText,
            OnDestroyDamageText,
            true,
            defaultCapacity,
            maxSize
        );
    }

    private DamageText CreateDamageText()
    {
        DamageText createdDamageText = Instantiate(
            damageTextPrefab,
            transform
        );

        createdDamageText.SetOwnerPool(this);

        createdDamageText.gameObject.SetActive(false);

        return createdDamageText;
    }

    private void OnGetDamageText(DamageText damageText)
    {
        if (damageText == null)
        {
            return;
        }

        damageText.ResetForReuse();
    }

    private void OnReleaseDamageText(DamageText damageText)
    {
        if (damageText == null)
        {
            return;
        }

        damageText.gameObject.SetActive(false);

        damageText.transform.SetParent(transform);
    }

    private void OnDestroyDamageText(DamageText damageText)
    {
        if (damageText == null)
        {
            return;
        }

        Destroy(damageText.gameObject);
    }

    public DamageText GetDamageText(Vector3 spawnPosition)
    {
        DamageText damageText =
            damageTextPool.Get();

        damageText.transform.position =
            spawnPosition;

        damageText.gameObject.SetActive(true);

        return damageText;
    }

    public void ReleaseDamageText(DamageText damageText)
    {
        if (damageText == null)
        {
            return;
        }

        damageTextPool.Release(damageText);
    }
}