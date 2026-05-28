using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;

    [Header("Defense")]
    public float defendingDamageRate = 0.3f;

    private int currentHealth;
    private PlayerWeaponController weaponController;

    private void Awake()
    {
        currentHealth = maxHealth;
        weaponController = GetComponent<PlayerWeaponController>();
    }

    public void TakeDamage(int damage)
    {
        if (weaponController != null && weaponController.IsDefending)
        {
            damage = Mathf.RoundToInt(damage * weaponController.CurrentDefenseDamageRate);
        }

        currentHealth -= damage;

        Debug.Log($"Player took {damage} damage. HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log($"Player healed {amount}. HP: {currentHealth}");
    }

    private void Die()
    {
        Debug.Log("Player died.");
    }
}
