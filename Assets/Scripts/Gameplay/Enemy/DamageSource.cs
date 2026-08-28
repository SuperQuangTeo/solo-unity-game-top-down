using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            return;
        }
        playerHealth.TakeDamage(damageAmount);
    }
}
