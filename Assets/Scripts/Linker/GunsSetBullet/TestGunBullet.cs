using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：
//***************************************** 
public class TestGunBullet
{
    private int hit=30;//伤害
    private float speed = 1f;//子弹移动速度
    private Transform gunTrans;//发射子弹的枪
    private GameObject bullet;//子弹
    public TestGunBullet (int hit, Transform gunTrans, GameObject bullet)
    {
        this.hit = hit;
        this.gunTrans = gunTrans;
        this.bullet = bullet;
    }
    /// <summary>
    /// 生成子弹
    /// </summary>
    public void PutBullet()
    {
        GameObject newBullet = Object.Instantiate(bullet, gunTrans.position, Quaternion.identity);
        newBullet.GetComponent<TestBullet>().InitialFace(gunTrans.forward);
        newBullet.GetComponent<TestBullet>().SetBullet(hit);
    }
}
