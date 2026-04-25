using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：绷带三级父类，继承自Medicine
//*************************
[CreateAssetMenu(fileName = "Bomdage", menuName = "LogicCat/BagObject/Bomdage")]
public class Bomdage : Madicine
{
    private int healValue = 20;//治疗值
    public override void Use()
    {
        PlayerHealth.Instance.Heal(healValue);
    }
}
