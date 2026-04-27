using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public GameObject healthBarPrefab;
    // 这里定义偏移量，方便在 Inspector 调整
    public Vector3 healthBarOffset = new Vector3(0, 15f, 0);

    private HealthBarUI currentHealthBarInstance;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            // 1. 实例化血条
            GameObject barObj = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity, this.transform);

            // 2. 获取血条UI脚本组件
            currentHealthBarInstance = barObj.GetComponent<HealthBarUI>();

            if (currentHealthBarInstance != null)
            {
                // 3. 【关键修改】告诉血条：你的目标是当前这个敌人！
                currentHealthBarInstance.target = this.transform;

                // 4. 初始化血量
                currentHealthBarInstance.SetHealth(currentHealth, maxHealth);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("敌人受到伤害，剩余血量：" + currentHealth);

        if (currentHealthBarInstance != null)
        {
            currentHealthBarInstance.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("敌人已死亡");

        // 销毁血条游戏物体
        if (currentHealthBarInstance != null)
        {
            Destroy(currentHealthBarInstance.gameObject);
        }

        // 销毁敌人自己
        Destroy(gameObject);
    }
}