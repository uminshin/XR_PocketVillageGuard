using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonMushroomAlly : MonoBehaviour
{
    [Header("Mushroom Settings")]
    public string targetTag = "Enemy";
    public float growTime = 1f;
    public float attackInterval = 1f;

    private int damage;
    private float lifeTime;
    private float attackRadius;

    private bool isGrown;
    private float nextAttackTime;

    public void Init(int damage, float lifeTime, float attackRadius)
    {
        this.damage = damage;
        this.lifeTime = lifeTime;
        this.attackRadius = attackRadius;

        Invoke(nameof(FinishGrow), growTime);
        Destroy(gameObject, lifeTime);
    }

    private void FinishGrow()
    {
        isGrown = true;
        Debug.Log("Poison mushroom grew.");
    }

    private void Update()
    {
        if (!isGrown) return;
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackInterval;

        EnemyHealth target = FindNearestEnemy();

        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log("Poison mushroom attacked Enemy.");
        }
    }

    private EnemyHealth FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRadius);

        EnemyHealth nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            if (!col.CompareTag(targetTag)) continue;

            EnemyHealth enemy = col.GetComponentInParent<EnemyHealth>();
            if (enemy == null) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
