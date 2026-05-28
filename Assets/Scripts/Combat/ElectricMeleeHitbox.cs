using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricMeleeHitbox : MonoBehaviour
{
    [Header("Electric Melee Settings")]
    public string targetTag = "Enemy";
    public float tickInterval = 0.2f;

    private int damage;
    private float duration;

    private readonly Dictionary<EnemyHealth, float> nextDamageTimes = new Dictionary<EnemyHealth, float>();

    public void Init(int damage, float duration)
    {
        this.damage = damage;
        this.duration = duration;

        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth == null) return;

        if (!nextDamageTimes.ContainsKey(enemyHealth))
        {
            nextDamageTimes[enemyHealth] = 0f;
        }

        if (Time.time >= nextDamageTimes[enemyHealth])
        {
            enemyHealth.TakeDamage(damage);
            nextDamageTimes[enemyHealth] = Time.time + tickInterval;
        }
    }
}
