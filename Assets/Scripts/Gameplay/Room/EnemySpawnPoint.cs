using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Enemy Spawn Configuration")]
    [SerializeField] private EnemyHealth enemyPrefab;

    public EnemyHealth EnemyPrefab => enemyPrefab;
}