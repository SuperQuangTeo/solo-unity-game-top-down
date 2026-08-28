using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game Data/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Health")]

    [SerializeField] private int maxHealth = 3;

    public int MaxHealth => maxHealth;
}
