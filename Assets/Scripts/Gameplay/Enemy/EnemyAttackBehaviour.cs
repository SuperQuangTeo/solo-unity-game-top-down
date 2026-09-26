using UnityEngine;

public abstract class EnemyAttackBehaviour : MonoBehaviour
{
    public abstract bool CanAttack(Transform playerTransform);

    public abstract void UpdateAttack(Transform playerTransform);

    public abstract void ResetAttack();
}