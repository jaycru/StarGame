using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InteractionManager : MonoBehaviour
{
    [Header("探测设置")]
    public float detectDistance = 3f;
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

        if (Physics.Raycast(ray, out hit, detectDistance, interactLayer))
        {
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

        if (container.itemsInObject.Count == 0)
        {
            ClearUI(); // 这个函数会把整个面板 SetActive(false)
            return;
        }
        // 清空旧条目
        foreach (Transform child in lootContent) Destroy(child.gameObject);

        // 生成新条目
        for (int i = 0; i < container.itemsInObject.Count; i++)
        {
            int index = i;
            LootItem item = container.itemsInObject[index];

            GameObject slot = Instantiate(itemSlotPrefab, lootContent);
            slot.GetComponentInChildren<TextMeshProUGUI>().text = $"{item.itemName} x{item.amount}";

            // 绑定拾取按钮
            slot.GetComponent<Button>().onClick.AddListener(() => {
                // 1. 加入玩家背包
                if(InventoryManager.Instance != null) 
                    InventoryManager.Instance.AddItem(item);

                // 2. 从箱子删除
                container.itemsInObject.RemoveAt(index);

                // 3. 递归刷新
                RefreshLootList(container);

                // 4. 如果捡完了，自动关闭
                if (container.itemsInObject.Count == 0) ClearUI();
            });
        }
    }
}