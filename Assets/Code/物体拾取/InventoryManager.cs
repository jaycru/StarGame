using UnityEngine;
using System.Collections.Generic;
using TMPro; // 必须引用，用于控制格子里的数量文字

public class InventoryManager : MonoBehaviour
{
    // 单例模式，全游戏唯一的背包管家
    public static InventoryManager Instance;

    [Header("UI 面板引用")]
    public GameObject inventoryPanel; // 整个背包 Canvas 下的 Panel
    public Transform gridParent;      // 那个带 Grid Layout Group 的 GridWindow
    public GameObject slotPrefab;     // 刚才做的 InventorySlot 预制体

    [Header("背包数据")]
    // 所有的物资都存在这个 List 里
    public List<LootItem> items = new List<LootItem>();

    void Awake()
    {
        // 确保单例唯一性
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // 初始时关闭背包界面
        if(inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // 监听 B 键开关背包
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    // --- 逻辑1：开关背包 ---
    public void ToggleInventory()
    {
        // 如果死亡界面或者暂停菜单开着，建议不允许开背包（可选逻辑）
        if (Time.timeScale == 0 && !inventoryPanel.activeSelf) return;

        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            RefreshUI(); // 每次打开时刷新格子显示
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            // Time.timeScale = 0f; // 如果你希望开背包时游戏暂停，取消这行注释
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            // Time.timeScale = 1f; // 配合上面的暂停使用
        }
    }

    // --- 逻辑2：捡东西时调用（之前拾取功能调用的就是这个） ---
    public void AddItem(LootItem newItem)
    {
        // 检查背包里是否已经有同名的东西
        LootItem existingItem = items.Find(x => x.itemName == newItem.itemName);

        if (existingItem != null)
        {
            existingItem.amount += newItem.amount; // 叠加数量
        }
        else
        {
            // 如果是新种类，存入列表（创建一个副本防止引用错误）
            items.Add(new LootItem { itemName = newItem.itemName, amount = newItem.amount });
        }

        Debug.Log($"背包已收到: {newItem.itemName} x{newItem.amount}");
        
        // 如果捡东西时背包正好开着，实时刷新一下
        if (inventoryPanel.activeSelf) RefreshUI();
    }

    // --- 逻辑3：将数据转化为 UI 格子 ---
    public void RefreshUI()
    {
        // 1. 先把旧的格子全部删掉（清空视觉效果）
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 根据数据列表，重新生成新的格子
        foreach (LootItem i in items)
        {
            GameObject newSlot = Instantiate(slotPrefab, gridParent);
            
            // 填充格子里的文字（假设你的预制体里有一个 TMP 文字）
            TextMeshProUGUI amountText = newSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (amountText != null)
            {
                amountText.text = i.amount.ToString();
            }
            
            // 以后在这里添加：根据名字显示对应的 Icon 图片
        }
    }
}
