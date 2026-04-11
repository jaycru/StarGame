using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：显示剩余弹药
//***************************************** 
public class AmmoText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private int maxBullets = 30;
    //private int bullets;
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null )
        {
            Debug.LogError("Error! Dismissed testMeshPro");
        }
        Display(maxBullets);
    }

    void Update()
    {

    } 

    public void Display(int bullets)
    {
        Debug.Log("Successfully enter display!");
        textMeshPro.text = bullets.ToString() + "/" + maxBullets.ToString();
    }

    public void SetMaxBullets(int maxBullets)
    {
        this.maxBullets = maxBullets;
    }
}
