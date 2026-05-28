using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealZone : MonoBehaviour
{
    [Header("Heal Zone Settings")]
    public float tickInterval = 1f;

    private int healAmount;
    private float duration;
    private float radius;

    private readonly Dictionary<PlayerHealth, float> nextPlayerHealTimes = new Dictionary<PlayerHealth, float>();
    private readonly Dictionary<RevivableAlly, float> nextAllyHealTimes = new Dictionary<RevivableAlly, float>();

    public void Init(int healAmount, float duration, float radius)
    {
        this.healAmount = healAmount;
        this.duration = duration;
        this.radius = radius;

        transform.localScale = new Vector3(radius * 2f, 0.1f, radius * 2f);

        Destroy(gameObject, duration);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHealPlayer(other);
        TryHealAlly(other);
    }

     private void TryHealPlayer(Collider other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player == null) return;

        if (!nextPlayerHealTimes.ContainsKey(player))
        {
            nextPlayerHealTimes[player] = 0f;
        }

        if (Time.time >= nextPlayerHealTimes[player])
        {
            player.Heal(healAmount);
            nextPlayerHealTimes[player] = Time.time + tickInterval;
        }
    }

    private void TryHealAlly(Collider other)
    {
        RevivableAlly ally = other.GetComponentInParent<RevivableAlly>();

        if (ally == null) return;
        if (ally.IsDowned) return;

        if (!nextAllyHealTimes.ContainsKey(ally))
        {
            nextAllyHealTimes[ally] = 0f;
        }

        if (Time.time >= nextAllyHealTimes[ally])
        {
            ally.Heal(healAmount);
            nextAllyHealTimes[ally] = Time.time + tickInterval;
        }
    }
}
