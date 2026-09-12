using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Dash")]

    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.6f;

    [Header("Health")]

    [SerializeField] private int maxHealth = 6;
    [SerializeField] private float invincibilityDuration = 0.5f;

    [Header("Shooting")]

    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float fireInterval = 0.25f;
    [SerializeField] private float projectileSpawnOffset = 0.5f;
    [SerializeField] private float projectileLifetime = 3f;

    public float MoveSpeed => moveSpeed;
    public float Acceleration => acceleration;
    public float Deceleration => deceleration;
    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCooldown;
    public float ProjectileSpeed => projectileSpeed;
    public float FireInterval => fireInterval;
    public float ProjectileSpawnOffset => projectileSpawnOffset;
    public float ProjectileLifetime => projectileLifetime;
    public int MaxHealth => maxHealth;
    public float InvincibilityDuration => invincibilityDuration;
}
