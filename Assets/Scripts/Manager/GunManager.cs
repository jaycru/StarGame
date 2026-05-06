using UnityEngine;

public class GunManager : MonoBehaviour
{
    public InventorySlotUI[] inventorySlotUIs;
    public static GunManager Instance;
    public GameObject Gun;

    private GUN gun;
    private int GlockBulletCount;
    private int AKBulletCount;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        RefreshInventorySlots();
    }

    public void Reload(int type, int nowBullet)
    {
        if (Gun == null)
        {
            Debug.LogWarning("GunManager: Gun is not assigned.");
            return;
        }

        GetAllBullets();

        int needBullet = 0;

        switch (type)
        {
            case 1:
                gun = Gun.GetComponent<Glock>();
                if (gun == null)
                {
                    Debug.LogWarning("GunManager: Glock component was not found on Gun.");
                    return;
                }

                needBullet = Mathf.Min(gun.GetMaxBullets() - nowBullet, GlockBulletCount);
                EmptyBullet(GlockBulletCount);
                GlockBulletCount -= needBullet;
                UpdateAmmoUI(GlockBulletCount, gun.GetMaxBullets());
                break;

            case 2:
                gun = Gun.GetComponent<TestGun>();
                if (gun == null)
                {
                    Debug.LogWarning("GunManager: TestGun component was not found on Gun.");
                    return;
                }

                needBullet = Mathf.Min(gun.GetMaxBullets() - nowBullet, AKBulletCount);
                EmptyBullet(AKBulletCount);
                AKBulletCount -= needBullet;
                UpdateAmmoUI(AKBulletCount, gun.GetMaxBullets());
                break;

            default:
                Debug.LogWarning("GunManager: unknown bullet type " + type);
                return;
        }

        needBullet = Mathf.Max(needBullet, 0);

        InventorySlotUI bulletSlot = GetBulletSlot(type);
        if (bulletSlot != null && InventoryManager.Instance != null && needBullet > 0)
        {
            InventoryManager.Instance.RemoveItemsFromSlot(bulletSlot, needBullet);
        }

        if (needBullet > 0)
        {
            gun.AddBullets(needBullet);
        }
    }

    private void EmptyBullet(int bullets)
    {
        if (bullets == 0)
        {
            Debug.Log("No reserve bullets available.");
        }
    }

    private InventorySlotUI GetBulletSlot(int type)
    {
        RefreshInventorySlots();

        if (inventorySlotUIs == null || inventorySlotUIs.Length == 0)
        {
            return null;
        }

        for (int i = 0; i < inventorySlotUIs.Length; i++)
        {
            InventorySlotUI slot = inventorySlotUIs[i];
            if (slot == null || slot.currentItemAsset == null)
            {
                continue;
            }

            if (IsBulletItem(slot.currentItemAsset, type))
            {
                return slot;
            }
        }

        return null;
    }

    private void GetAllBullets()
    {
        GlockBulletCount = 0;
        AKBulletCount = 0;

        InventorySlotUI glockBulletSlot = GetBulletSlot(1);
        if (glockBulletSlot != null)
        {
            GlockBulletCount = glockBulletSlot.CurrentCount;
        }

        InventorySlotUI akBulletSlot = GetBulletSlot(2);
        if (akBulletSlot != null)
        {
            AKBulletCount = akBulletSlot.CurrentCount;
        }
    }

    private void RefreshInventorySlots()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        if (InventoryManager.Instance.allSlots != null && InventoryManager.Instance.allSlots.Length > 0)
        {
            inventorySlotUIs = InventoryManager.Instance.allSlots;
        }
    }

    private bool IsBulletItem(ItemAsset item, int type)
    {
        if (item == null)
        {
            return false;
        }

        if (item.name == "bullet" + type)
        {
            return true;
        }

        if (type == 1 && item.itemId == "glock_bullet")
        {
            return true;
        }

        if (type == 2 && item.itemId == "ak_bullet")
        {
            return true;
        }

        return false;
    }

    private void UpdateAmmoUI(int current, int max)
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateAmmo(current, max);
        }
    }
}
