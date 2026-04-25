using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glock : MonoBehaviour
{
    private float waitTime = 0.2f;//从按下左键到开火的间隔时间
    private int maxBullets = 20;
    private int nowBullets = 20;
    private int hit;
    private float reloadTime;
    private Coroutine shoot = null;
    private Coroutine reload = null;
    private bool isShooting = false;
    private bool isReloading = false;
    private AmmoText ammoTextComponent;
    public GameObject bullet;//子弹（临时用公共）
    public GameObject bulletMouth;//弹口（临时用公共） 
    public GameObject ammoText;//弹药显示UI

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (ShouldBlockWeaponInput())
        {
            return;
        }

        ControlDirection();
        ControlFire();
        ControlReload();
    }

    private bool ShouldBlockWeaponInput()
    {
        return Time.timeScale == 0f
            || Cursor.lockState != CursorLockMode.Locked
            || Cursor.visible
            || InteractionManager.IsLootListOpen;
    }

    private void Initialize()
    {
        Debug.Log("Initialize");
        //初始化弹药UI显示
        ammoTextComponent = ammoText.GetComponent<AmmoText>();
    }

    private void ControlDirection()
    {
        Camera mainCamera = Camera.main;
        transform.rotation = mainCamera.transform.rotation;
        bulletMouth.transform.rotation = mainCamera.transform.rotation;
    }

    private void ControlFire()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading && nowBullets != 0 && !isShooting)
        {
            StartFire();
        }
    }

    private void ControlReload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            StartReload();
        }
    }

    private void StartFire()
    {
        shoot = StartCoroutine("DoShoot");
    }

    private IEnumerator DoShoot()
    {   
        isShooting = true;
        TestGunBullet testGunBullet = new TestGunBullet(hit, bulletMouth.transform, bullet);
        testGunBullet.PutBullet();
        nowBullets--;
        ammoTextComponent.Display(nowBullets);
        testGunBullet = null;
        Debug.Log("Shooting! now Bullets are : " + nowBullets);
        yield return new WaitForSeconds(waitTime);
        isShooting=false;
    }

    private void StartReload()
    {
        if (reload == null && !isShooting)
        {
            reload = StartCoroutine("DoReload");
        }
    }

    private IEnumerator DoReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        nowBullets = maxBullets;
        ammoTextComponent.Display(maxBullets);
        Debug.Log("Reloading! now Bullets are : " + nowBullets + "equals to " + maxBullets);
        reload = null;
    }
}
