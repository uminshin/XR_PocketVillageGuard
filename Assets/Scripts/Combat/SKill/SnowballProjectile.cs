using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballProjectile : MonoBehaviour
{
    [Header("Snowball Settings")]
    public float lifeTime = 5f;
    public string targetTag = "Enemy";

    [Header("Freeze")]
    public float freezeDuration = 3f;

    private int damage;
    private bool hasHit;

    public void Init(int damage)
    {
        this.damage = damage;
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        if (!collision.collider.CompareTag(targetTag))
        {
            Destroy(gameObject);
            return;
        }

        hasHit = true;

        EnemyHealth enemyHealth = collision.collider.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        EnemyStatus enemyStatus = collision.collider.GetComponentInParent<EnemyStatus>();

        if (enemyStatus != null)
        {
            enemyStatus.Freeze(freezeDuration);
        }
        else
        {
            Debug.LogWarning("EnemyStatus script not found on hit object.");
        }

        Destroy(gameObject);
    }
}
