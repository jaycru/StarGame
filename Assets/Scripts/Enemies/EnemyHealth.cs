using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： pjjay
//功能说明：敌军生命
//***************************************** 
public class EnemyHealth : MonoBehaviour
{
    [Header("UI")]
    public Slider healthBar;
    public Text healthText;

    [Header("颜色")]
    public Color fullHealthColor = Color.red;
    public Color lowHealthColor = Color.green;

    [Header("数据")]
    public float maxHealth = 100f;

    public float currentHealth;
    //public PlayerController playerController;
    //TODO：这是什么东西？拿的什么引用？起什么作用？？

    private void Start()
    {
        //playerController = GetComponent<PlayerController>();

        //if (playerController == null)
        //{
        //    Debug.LogError("当前物体无PlayerController脚本");
        //    return;
        //}

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            if (healthBar.GetComponentInChildren<Image>())
                healthBar.GetComponentInChildren<Image>().color = fullHealthColor;

        }

        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = Mathf.CeilToInt(currentHealth) + "/" + maxHealth;
        }

        if (healthBar != null)
        {
            Image fillImage = healthBar.GetComponentInChildren<Image>();
            if (fillImage != null)
            {
                if (currentHealth <= maxHealth * 0.3f)
                {
                    fillImage.color = lowHealthColor;
                }
                else
                {
                    fillImage.color = fullHealthColor;
                }
            }
        }
    }
}
