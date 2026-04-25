using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：玩家血量系统
//*************************

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update
    private int playerHitPoints;
    private int maxHP = 100;
    void Start()
    {
        playerHitPoints = maxHP;
    }

    // Update is called once per frame
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
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player has died.");
    }
}
