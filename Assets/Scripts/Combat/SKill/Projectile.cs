using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifeTime = 5f;
    public string targetTag = "Enemy";

    private int damage;
    private float speed;
    private Vector3 moveDirection;

    public void Init(int damage, float speed, Vector3 direction)
    {
        this.damage = damage;
        this.speed = speed;
        this.moveDirection = direction.normalized;

        //Debug.Log($"Projectile initialized. Damage: {damage}, Speed: {speed}, Direction: {moveDirection}");

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag))
        {
            return;
        }

        // Debug.Log($"Projectile hit Enemy: {other.gameObject.name}");

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
        else
        {
            Debug.LogWarning("EnemyHealth script not found on hit object.");
        }

        Destroy(gameObject);
    }
}
