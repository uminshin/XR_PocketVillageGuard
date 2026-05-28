using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponElement
{
    Gun,
    Fire,
    Ice,
    Electric,
    Poison
}

public enum AbilityType
{
    None,
    Projectile,
    Barrier,
    Bomb,
    FlameWall,
    Snowball,
    IceShield,
    ElectricMelee,
    ElectricReviveChannel
}

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Combat/Weapon Data")]
public class WeaponData : ScriptableObject
{
     [Header("Basic")]
    public string weaponName;
    public WeaponElement element;
    public GameObject weaponModelPrefab;

    [Header("Attack")]
    public AbilityType attackType = AbilityType.Projectile;
    public GameObject attackPrefab;
    public int attackDamage = 10;
    public float attackCooldown = 0.5f;
    public float attackSpeed = 10f;
    public float attackRadius = 3f;
    public float attackDuration = 0.5f;

    [Header("Defense")]
    public AbilityType defenseType = AbilityType.Barrier;
    public GameObject defensePrefab;
    public int defensePower = 10;
    public float defenseCooldown = 1f;
    public float defenseDuration = 3f;
    public float defenseRadius = 2f;
    public float defenseDamageRate = 0.3f;
}
