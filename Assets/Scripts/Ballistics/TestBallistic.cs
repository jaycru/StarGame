using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：测试弹道逻辑
//方程：y(n) = a*n^b+c*sin(d*n)
//     x(n) = e*f^n*sin(g*n)
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
    void Start()
    {

    }

    void Update()
    {

    }
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
    public void BallisticDeviation(int n)
    {
        float x = verticalLiftStrength * Mathf.Pow(n, verticalGrowthIndex)
            + verticalSinus * Mathf.Sin(verticalFrequency * n);
        float y = horizontalInitialSwing * Mathf.Pow(horizontalDamping, n)
            * Mathf.Sin(horizontalFrequency * n);
        Camera mainCamera = Camera.main;
        mainCamera.transform.localRotation = Quaternion.Euler
            (mainCamera.transform.localEulerAngles.x - y, mainCamera.transform.localEulerAngles.y + x, 0);
    }
}
