using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    public float lifeTime = 5f;
    public float explosionDelay = 0f;
    public GameObject explosionEffect;
    public LayerMask enemyLayer;

    private int damage;
    private float explosionRadius;
    private bool hasExploded;

    public void Init(int damage, float explosionRadius)
    {
        this.damage = damage;
        this.explosionRadius = explosionRadius;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        Debug.Log($"Bomb exploded. Radius: {explosionRadius}, Damage: {damage}");

        Destroy(gameObject);
    }
}
