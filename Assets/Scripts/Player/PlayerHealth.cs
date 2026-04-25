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
    public static PlayerHealth Instance;
    // Start is called before the first frame update
    private int playerHitPoints;
    private int maxHP = 100;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        playerHitPoints = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20); // 按H键测试受伤
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Heal(20); // 按J键测试治疗
        }
    }

    public void TakeDamage(int damageAmount)
    {
        playerHitPoints = Mathf.Max(playerHitPoints - damageAmount, 0);
        PlayerToUIManager.Instance.ChangeHealth(playerHitPoints, maxHP, false);
        if (playerHitPoints <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        if (playerHitPoints < maxHP)
        {
            playerHitPoints = Mathf.Min(playerHitPoints + healAmount, maxHP);
            PlayerToUIManager.Instance.ChangeHealth(playerHitPoints, maxHP, true);
        }
    }

    private void Die()
    {
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player has died.");
    }
}
