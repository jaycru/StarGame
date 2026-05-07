using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    public static bool IsLootListOpen { get; private set; }
    public static bool WasLootListClosedThisFrame => lastLootListCloseFrame == Time.frameCount;
    private static int lastLootListCloseFrame = -1;

    [Header("探测设置")]
    public float detectDistance;
    public LayerMask interactLayer;

    [Header("UI 引用")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public GameObject lootUI;
    public Transform lootContent;
    public GameObject itemSlotPrefab;

    private Interactable lastTarget; // 记录上一次指着的物体，防止每一帧重复刷新列表
    private bool isLootListOpen;

    void Update()
    {
        if (isLootListOpen)
        {
            if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                lastTarget = null;
                ClearUI();
                return;
            }

            HandleLootHotkeys();
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * detectDistance, Color.red);

        if (Physics.Raycast(ray, out hit, detectDistance, interactLayer))
        {
            Debug.Log("Successfully Ray!");
            Interactable obj = hit.collider.GetComponent<Interactable>();
            if (obj != null)
            {
                // 如果指着的是新物体，或者是第一次指着
                if (obj != lastTarget)
                {
                    lastTarget = obj;
                    UpdateUI(obj);
                }

                // 监听交互按键
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (obj.type == InteractType.LootList)
                    {
                        OpenLootList(obj);
                    }
                    else
                    {
                        obj.DoAction();
                    }
                }
                return;
            }
        }

        // 射线没射中任何 Interactable 物体，执行清理
        if (lastTarget != null)
        {
            lastTarget = null;
            ClearUI();
        }
    }

    void UpdateUI(Interactable target)
    {
        if (target.type == InteractType.Simple)
        {
            isLootListOpen = false;
            IsLootListOpen = false;
            promptUI.SetActive(true);
            lootUI.SetActive(false);
            promptText.text = "[F] " + target.simpleActionName;

            // 简单交互不需要鼠标，确保它锁好
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (target.type == InteractType.LootList)
        {
            isLootListOpen = false;
            IsLootListOpen = false;
            promptUI.SetActive(true);
            lootUI.SetActive(false);
            promptText.text = "[F] 搜索物资";

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OpenLootList(Interactable target)
    {
        if (target.itemsInObject.Count == 0)
        {
            ClearUI();
            return;
        }

        lastTarget = target;
        isLootListOpen = true;
        IsLootListOpen = true;
        promptUI.SetActive(false);
        lootUI.SetActive(true);

        // 只有主动打开物资列表时，才释放鼠标让玩家点
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshLootList(target);
    }

    // 清理函数：当准星移开时调用
    void ClearUI()
    {
        bool wasLootListOpen = isLootListOpen || IsLootListOpen;
        isLootListOpen = false;
        IsLootListOpen = false;
        if (wasLootListOpen) lastLootListCloseFrame = Time.frameCount;

        promptUI.SetActive(false);
        lootUI.SetActive(false);

        // 【关键】UI消失时，必须把鼠标重新锁回屏幕中心，否则玩家没法继续转头
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshLootList(Interactable container)
    {
        // 1. 如果容器空了，关闭 UI
        if (container.itemsInObject.Count == 0)
        {
            ClearUI();
            return;
        }

        // 2. 清空旧条目
        foreach (Transform child in lootContent) Destroy(child.gameObject);

        // 3. 重新生成条目
        for (int i = 0; i < container.itemsInObject.Count; i++)
        {
            Debug.Log("Enter circle");
            LootItem item = container.itemsInObject[i];

            // 防止数据为空导致崩溃
            if (item.details == null) continue;

            GameObject slot = Instantiate(itemSlotPrefab, lootContent);

            // --- 修复报错 1：访问 details 里的名字 ---
            string prefix = i < 9 ? $"[{i + 1}] " : "";
            slot.GetComponentInChildren<TextMeshProUGUI>().text = $"{prefix}{item.details.itemName} x{item.amount}";

            // 绑定按钮
            slot.GetComponent<Button>().onClick.AddListener(() => {
                TryTakeLootItem(container, item);
            });
        }
    }

    private void HandleLootHotkeys()
    {
        if (lastTarget == null) return;

        int maxHotkeyCount = Mathf.Min(lastTarget.itemsInObject.Count, 9);
        for (int i = 0; i < maxHotkeyCount; i++)
        {
            KeyCode alphaKey = (KeyCode)((int)KeyCode.Alpha1 + i);
            KeyCode keypadKey = (KeyCode)((int)KeyCode.Keypad1 + i);

            if (Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey))
            {
                TryTakeLootItem(lastTarget, lastTarget.itemsInObject[i]);
                return;
            }
        }
    }

    private void TryTakeLootItem(Interactable container, LootItem item)
    {
        if (container == null || item == null || item.details == null) return;

        bool added = false;
        if (InventoryManager.Instance != null)
        {
            added = InventoryManager.Instance.AddItem(item.details, item.amount);
        }
        else
        {
            Debug.LogWarning("拾取失败：场景中找不到 InventoryManager！");
        }

        if (!added)
        {
            string reason = InventoryManager.Instance != null
                ? InventoryManager.Instance.LastAddItemFailureReason
                : "场景中找不到 InventoryManager";

            Debug.LogWarning($"拾取失败：{item.details.itemName} 未加入背包，物品保留在箱子中。原因：{reason}");
            return;
        }

        container.itemsInObject.Remove(item);
        RefreshLootList(container);

        if (container.itemsInObject.Count == 0) ClearUI();
    }
}
