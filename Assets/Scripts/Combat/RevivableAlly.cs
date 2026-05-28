using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivableAlly : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;
    public int revivedHealth = 25;

    [Header("Revive")]
    public bool startDowned = true;
    public float requiredReviveTime = 3f;

    public bool IsDowned { get; private set; }

    private int currentHealth;
    private float reviveProgress;
    private float nextLogTime;

    private void Awake()
    {
        if (startDowned)
        {
            Down();
        }
        else
        {
            currentHealth = maxHealth;
            IsDowned = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (IsDowned) return;

        currentHealth -= damage;

        Debug.Log($"{gameObject.name} ally took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Down();
        }
    }

    public void Down()
    {
        currentHealth = 0;
        reviveProgress = 0f;
        IsDowned = true;

        Debug.Log($"{gameObject.name} is downed.");
    }

    public void AddReviveProgress(float amount)
    {
        if (!IsDowned) return;

        reviveProgress += amount;

        if (Time.time >= nextLogTime)
        {
            Debug.Log($"Reviving {gameObject.name}: {reviveProgress:F1}/{requiredReviveTime:F1}");
            nextLogTime = Time.time + 1f;
        }

        if (reviveProgress >= requiredReviveTime)
        {
            Revive();
        }
    }

    private void Revive()
    {
        IsDowned = false;
        currentHealth = revivedHealth;
        reviveProgress = 0f;

        Debug.Log($"{gameObject.name} revived. HP: {currentHealth}");
    }
}
