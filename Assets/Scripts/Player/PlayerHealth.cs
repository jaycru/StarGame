using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private int playerHitPoints;
    private int maxHP = 100;

    void Start()
    {
        playerHitPoints = maxHP;
    }

    void Update()
    {
    }

    public void TakeDamage(int damageAmount)
    {
        playerHitPoints = Mathf.Max(playerHitPoints - damageAmount, 0);
        if (playerHitPoints <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        playerHitPoints = Mathf.Min(playerHitPoints + healAmount, maxHP);
    }

    private void Die()
    {
        Debug.Log("Player has died.");
    }
}
