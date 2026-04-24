using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：测试弹道逻辑
//方程：x(n) = a*n^b+c*sin(d*n)
//     y(n) = e*f^n*sin(g*n)
//***************************************** 
public class TestBallistic : MonoBehaviour
{
    private float verticalLiftStrength;//垂直上抬强度a
    private float verticalGrowthIndex;//垂直增长指数b
    private float verticalSinus;//垂直正弦波动幅度c
    private float verticalFrequency;//垂直波动频率d
    private float horizontalInitialSwing;//水平摆动起始幅度e
    private float horizontalDamping;//水平摆动衰减系数f
    private float horizontalFrequency;//水平摆动频率g

    private Quaternion initialRotation;
    /// <summary>
    /// 初始化七大参数
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <param name="f"></param>
    /// <param name="g"></param>
    public void SetBallistic(float a, float b, float c, float d, float e, float f, float g)
    {
        verticalLiftStrength = a;
        verticalGrowthIndex = b;
        verticalSinus = c;
        verticalFrequency = d;
        horizontalInitialSwing = e;
        horizontalDamping = f;
        horizontalFrequency = g;
    }
    /// <summary>
    /// 弹道偏移
    /// </summary>
    /// <param name="n"></param>
    public Vector2 BallisticDeviation(int n)
    {
        float x = verticalLiftStrength * Mathf.Pow(n, verticalGrowthIndex)
            + verticalSinus * Mathf.Sin(verticalFrequency * n);//垂直偏移
        float y = horizontalInitialSwing * Mathf.Pow(horizontalDamping, n)
            * Mathf.Sin(horizontalFrequency * n);//水平偏移
        Debug.Log("The Horizontal movement is :" + y + " The Vertical movement is :" + x);
        //GameObject gun = this.gameObject;
        //gun.transform.localRotation = gun.transform.localRotation * Quaternion.Euler(-x, y, 0);//修改：以现在作为朝向做偏移
        Vector2 direction = new Vector2(x, y);
        return direction;
        //gun.transform.localRotation = Quaternion.Euler
            //(gun.transform.localEulerAngles.x - x, gun.transform.localEulerAngles.y + y, 0);
    }
}
