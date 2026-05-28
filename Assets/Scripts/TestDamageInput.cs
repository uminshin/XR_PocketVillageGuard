using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageInput : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public int testDamage = 20;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Test damage input detected.");

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(testDamage);
            }
            else
            {
                Debug.LogWarning("PlayerHealth is not assigned.");
            }
        }
    }
}
