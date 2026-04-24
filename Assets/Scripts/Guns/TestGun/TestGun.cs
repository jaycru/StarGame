using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：测试用枪
//***************************************** 
public class TestGun : MonoBehaviour
{
    private int maxBullets = 30;//最大弹匣容量
    private int nowBullets = 30;//当前子弹数
    private int hit = 30;//子弹伤害
    private float shootSpeed = 0.15f;//射速（射击间隔）
    private float reloadTime = 1f;//换弹时间
    private Coroutine Shoot = null;//发射子弹协程
    private Coroutine Reload = null;//换弹协程 
    private bool isShooting;//是否在发射子弹
    private bool isReloading;//是否在换弹
    private Vector2 lastDirection;//系统记忆的上一个枪身偏移位
    private AmmoText ammoTextComponent;//弹药显示UI脚本
    public TestBallistic testBallistic;//弹道偏移脚本
    public GameObject bullet;//子弹（临时用公共）
    public GameObject bulletMouth;//弹口（临时用公共） 
    public GameObject ammoText;//弹药显示UI
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        ControlDirection();
        ControlFire();
        ControlReload();
    }
    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialize()
    {
        Debug.Log("Initialize");
        //初始化弹药UI显示
        ammoTextComponent = ammoText.GetComponent<AmmoText>();
        //初始化弹道偏移方程
        testBallistic = this.AddComponent<TestBallistic>();
        testBallistic.SetBallistic(0.14f, 1.3f, 0.03f, 0.8f, 0.5f, 0.96f, 1.0f);
    }
    /// <summary>
    /// 操控枪口朝向，即枪口初始永远朝向屏幕正中，然后根据开枪状态调整枪口偏移
    /// </summary>
    public void ControlDirection()
    {
        Camera mainCamera = Camera.main;
        transform.rotation = mainCamera.transform.rotation;
        bulletMouth.transform.rotation = mainCamera.transform.rotation;
        if (isShooting)
        {
            GameObject gun = this.gameObject;
            float x = lastDirection.x;
            float y = lastDirection.y;
            gun.transform.localRotation = gun.transform.localRotation * Quaternion.Euler(-x, y, 0);
            bulletMouth.transform.localRotation = bulletMouth.transform.localRotation * Quaternion.Euler(-x, y, 0);
        }
    }
    /// <summary>
    /// 操控开火流程
    /// </summary>
    public void ControlFire()
    {
        if (Input.GetMouseButton(0) && nowBullets != 0)
        {
            StartFire();
        }
        else if (Input.GetMouseButtonUp(0) || nowBullets == 0)
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
        int n = 0;
        while (isShooting)
        {
            Debug.Log("n =" + n);
            //处理弹道偏移
            lastDirection = testBallistic.BallisticDeviation(n);
            //创建子弹
            TestGunBullet testGunBullet = new TestGunBullet(hit, bulletMouth.transform, bullet);
            testGunBullet.PutBullet();
            testGunBullet = null;
            //循环更新数据
            n++;
            nowBullets--;
            ammoTextComponent.Display(nowBullets);
            //打印调试信息
            Debug.Log("Shooting! now Bullets are : " + nowBullets);
            //返回间隔时间
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
        ammoTextComponent.Display(maxBullets);
        Debug.Log("Reloading! now Bullets are : " + nowBullets + "equals to " + maxBullets);
        Reload = null;
    }

    /// <summary>
    /// 标准化旋转
    /// </summary>
    public void StandardizeRotation()
    {

    }
}
