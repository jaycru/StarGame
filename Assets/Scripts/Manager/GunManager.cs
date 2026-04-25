using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：管理枪械核心功能，如换弹与子弹数量的关联
//*************************

public class GunManager : MonoBehaviour
{
    public InventorySlotUI[] inventorySlotUIs; 
    public static GunManager Instance;//单例模式
    public GameObject Gun;//枪械对象
    private GUN gun;//枪械脚本
    private int GlockBulletCount;//格洛克子弹数量
    private int AKBulletCount;//AK子弹数量
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 换弹
    /// </summary>
    /// <param name="type">枪械类型，1为格洛克，2为AK</param>
    /// <param name="nowBullet">当前子弹数量</param>
    public void Reload(int type, int nowBullet)
    {
        int needBullet;
        //更新所有子弹数量
        GetAllBullets();
        //根据枪械类型获取枪械脚本，更新子弹数量
        switch (type)
        {
            case 1:
                gun = Gun.GetComponent<Glock>();
                needBullet = Mathf.Min(gun.GetMaxBullets() - nowBullet, GlockBulletCount);
                EmptyBullet(GlockBulletCount);
                GlockBulletCount -= needBullet;
                HUDManager.Instance.UpdateAmmo(GlockBulletCount, gun.GetMaxBullets());
                break;
            case 2:
                gun = Gun.GetComponent<TestGun>();
                needBullet = Mathf.Min(gun.GetMaxBullets() - nowBullet, AKBulletCount);
                EmptyBullet(AKBulletCount);
                AKBulletCount -= needBullet;
                HUDManager.Instance.UpdateAmmo(AKBulletCount, gun.GetMaxBullets());
                break;
            default:
                needBullet = 0;
                break;
        }
        //将剩余子弹数量更新到UI显示
        InventorySlotUI inventorySlotUI = GetBulletSlot(type);
        if (inventorySlotUI != null)
        {
            InventoryManager.Instance.RemoveItemsFromSlot(inventorySlotUI, needBullet);
        }
        //具体枪械增加子弹数量
        gun.AddBullets(needBullet);
    }

    private void EmptyBullet(int bullets)
    {
        if (bullets == 0)
        {
            Debug.Log("备用载弹量不足，无法换弹");
        }
    }
    /// <summary>
    /// 拿到子弹槽UI组件，以便更新子弹数量显示
    /// </summary>
    /// <returns></returns>
    private InventorySlotUI GetBulletSlot(int type)
    {
        InventorySlotUI inventorySlotUI;
        for (int i = 0; i < inventorySlotUIs.Length; i++)
        {
            if (inventorySlotUIs[i].currentItemAsset != null)
            {
                ItemAsset item = inventorySlotUIs[i].currentItemAsset;
                if (item.name == $"bullet{type}")
                {
                    inventorySlotUI = inventorySlotUIs[i];
                    return inventorySlotUI;
                }
            }
        }
        return null;
    }

    private void GetAllBullets()
    {
        if (GetBulletSlot(1) != null)
        {
            GlockBulletCount = GetBulletSlot(1).CurrentCount;
        }
        if (GetBulletSlot(2) != null)
        {
            AKBulletCount = GetBulletSlot(2).CurrentCount;
        }
    }
}
