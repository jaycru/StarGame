using UnityEngine;
using System.Collections.Generic;
using TMPro; // 必须引用，用于解析格子里的数字文本

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI 引用")]
    public GameObject inventoryPanel;
    public Transform gridParent;       // 刚才报错的变量，现在已补全

    [Header("内部格位名单")]
    public InventorySlotUI[] allSlots; // 系统启动时会自动填满这个名单

    void Awake()
    {
        // 初始化单例，方便拾取脚本调用
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 1. 游戏开始时隐藏背包
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        // 2. 自动搜索 gridParent 下所有的格子脚本并存入名单
        if (gridParent != null)
        {
            allSlots = gridParent.GetComponentsInChildren<InventorySlotUI>();
            Debug.Log($"背包系统初始化成功：共识别到 {allSlots.Length} 个存储格。");
        }
        else
        {
            Debug.LogError("错误：请在 Inspector 面板将 GridWindow 物体拖入 Grid Parent 槽位！");
        }
    }

    void Update()
    {
        // 监听 B 键开关背包
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    // --- 逻辑：开关控制 ---
    public void ToggleInventory()
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            // 打开背包：释放鼠标，显示指针
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Time.timeScale = 0f; // 如果你想让背包打开时游戏暂停，取消这行注释
        }
        else
        {
            // 关闭背包：重新锁定鼠标
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Time.timeScale = 1f;
        }
    }

    public void AddItem(ItemAsset asset, int count)
    {
        // 1. 【堆叠逻辑】比对身份卡，而不是比对图片
        foreach (InventorySlotUI slot in allSlots)
        {
            // 核心修改：判断格子里存的“身份卡”是否等于正要捡的“身份卡”
            if (slot.isFull && slot.currentItemAsset == asset)
            {
                int currentCount = int.Parse(slot.amountText.text);
                slot.SetItem(asset, currentCount + count);
                Debug.Log($"{asset.itemName} 堆叠成功。");
                return;
            }
        }

        // 2. 【寻找空位逻辑】保持不变
        foreach (InventorySlotUI slot in allSlots)
        {
            if (!slot.isFull)
            {
                slot.SetItem(asset, count);
                return;
            }
        }
        Debug.LogWarning("背包已满！");
    }
}
