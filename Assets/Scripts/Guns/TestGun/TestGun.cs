using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：测试用枪
//***************************************** 
public class TestGun : MonoBehaviour
{
    private int maxBullets = 30;//最大弹匣容量
    private int nowBullets = 20;//当前子弹数
    private float shootSpeed = 1f;//射速（设计间隔）
    private float reloadTime = 1f;//换弹时间
    private Coroutine Shoot = null;//发射子弹协程
    private Coroutine Reload = null;//换弹协程 
    private bool isShooting;//是否在发射子弹
    private bool isReloading;//是否在换弹
    void Start()
    {

    }
    void Update()
    {
        ControlFire();
        ControlReload();
    }
    /// <summary>
    /// 操控开火流程
    /// </summary>
    public void ControlFire()
    {
        if (Input.GetMouseButton(0))
        {
            StartFire();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndFire();
        }
    }
    public void ControlReload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            StartReload();
        }
    }
    /// <summary>
    /// 开火
    /// </summary>
    public void StartFire()
    {
        isShooting = true;
        if (Shoot == null)
        {
            Shoot = StartCoroutine("DoShoot");
        }
    }
    /// <summary>
    /// 停火
    /// </summary>
    public void EndFire()
    {
        isShooting = false;
    }
    /// <summary>
    /// 开始换弹
    /// </summary>
    public void StartReload()
    {
        if (Reload == null && !isShooting)
        {
            Reload = StartCoroutine("DoReload");
        }
    }
    /// <summary>
    /// 发射子弹协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoShoot()
    {
        while (isShooting)
        {
            nowBullets--;
            Debug.Log("Shooting! now Bullets are : " + nowBullets);
            yield return new WaitForSeconds(shootSpeed);
        }
        Shoot = null;
        yield return null;
    }
    /// <summary>
    /// 换弹协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoReload()
    {
        yield return new WaitForSeconds(reloadTime);
        nowBullets = maxBullets;
        Debug.Log("Reloading! now Bullets are : " + nowBullets + "equals to " + maxBullets);
    }
}
