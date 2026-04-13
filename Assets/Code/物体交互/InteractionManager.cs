using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
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

    void Update()
    {
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
                if (Input.GetKeyDown(KeyCode.F)) obj.DoAction();
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
            promptUI.SetActive(true);
            lootUI.SetActive(false);
            promptText.text = "[F] " + target.simpleActionName;

            // 简单交互不需要鼠标，确保它锁好
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (target.type == InteractType.LootList)
        {
            promptUI.SetActive(false);
            lootUI.SetActive(true);

            // 【关键】只有看到物资列表时，才释放鼠标让玩家点
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            RefreshLootList(target);
        }
    }

    // 清理函数：当准星移开时调用
    void ClearUI()
    {
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
            int index = i;
            LootItem item = container.itemsInObject[index];

            // 防止数据为空导致崩溃
            if (item.details == null) continue;

            GameObject slot = Instantiate(itemSlotPrefab, lootContent);

            // --- 修复报错 1：访问 details 里的名字 ---
            slot.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.details.itemName} x{item.amount}";

            // 绑定按钮
            slot.GetComponent<Button>().onClick.AddListener(() => {

                // --- 修复报错 2：传入 asset 和 amount 两个参数 ---
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.AddItem(item.details, item.amount);
                }

                container.itemsInObject.RemoveAt(index);
                RefreshLootList(container);

                if (container.itemsInObject.Count == 0) ClearUI();
            });
        }
    }
}