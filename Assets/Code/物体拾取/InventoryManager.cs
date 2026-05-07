using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI")]
    public GameObject inventoryPanel;
    public Transform gridParent;

    [Header("Slots")]
    public InventorySlotUI[] allSlots;

    public string LastAddItemFailureReason { get; private set; }

    private InventorySlotUI selectedSlot;
    private GameObject itemActionBlocker;
    private GameObject itemActionPanel;
    private RectTransform itemActionPanelRect;
    private TextMeshProUGUI itemNameText;
    private TextMeshProUGUI itemCountText;
    private Button useButton;
    private Button dropButton;
    private TextMeshProUGUI useButtonLabel;
    private TextMeshProUGUI dropButtonLabel;

    private readonly Color panelColor = new Color(0.08f, 0.1f, 0.12f, 0.96f);
    private readonly Color buttonColor = new Color(0.92f, 0.92f, 0.92f, 1f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("场景中存在多个 InventoryManager，已使用当前激活的背包管理器。");
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshSlots();

        if (allSlots != null && allSlots.Length > 0)
        {
            Debug.Log($"背包系统初始化成功：共识别到 {allSlots.Length} 个存储格。");
        }
        else
        {
            Debug.LogError("背包系统初始化失败：没有识别到任何 InventorySlotUI。请检查 Grid Parent 是否指向 GridWindow，并确认每个背包格子都挂有 InventorySlotUI 脚本。");
        }

        EnsureItemActionPanel();
        HideItemActionPanel();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);
        HideItemActionPanel();

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }

        if (isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
        }
    }

    public bool AddItem(ItemAsset asset, int count)
    {
        LastAddItemFailureReason = string.Empty;

        if (asset == null)
        {
            LastAddItemFailureReason = "物品数据为空";
            Debug.LogWarning("拾取失败：物品数据为空！");
            return false;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"拾取数量无效：{asset.itemName} 的数量是 {count}，已按 1 个处理。");
            count = 1;
        }

        RefreshSlots();

        if (allSlots == null || allSlots.Length == 0)
        {
            LastAddItemFailureReason = "背包格子没有初始化，检查 InventoryManager 的 Grid Parent 是否指向 GridWindow";
            Debug.LogWarning("拾取失败：背包格子没有初始化！");
            return false;
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null) continue;
            NormalizeSlotState(slot);

            if (slot.HasItem && IsSameInventoryItem(slot.currentItemAsset, asset))
            {
                slot.SetItem(asset, slot.CurrentCount + count);
                Debug.Log($"{asset.itemName} 堆叠成功。");
                return true;
            }
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null) continue;
            NormalizeSlotState(slot);

            if (!slot.HasItem)
            {
                slot.SetItem(asset, count);
                Debug.Log($"{asset.itemName} 已放入背包。");
                return true;
            }
        }

        LastAddItemFailureReason = "没有可用空格子，也没有可堆叠的同类物品。" + BuildSlotDebugSummary();
        Debug.LogWarning($"背包已满，无法拾取 {asset.itemName}。{BuildSlotDebugSummary()}");
        return false;
    }

    private void RefreshSlots()
    {
        if (gridParent != null)
        {
            InventorySlotUI[] slotsInGrid = gridParent.GetComponentsInChildren<InventorySlotUI>(true);
            if (slotsInGrid != null && slotsInGrid.Length > 0)
            {
                allSlots = slotsInGrid;
                return;
            }
        }

        if (inventoryPanel != null)
        {
            InventorySlotUI[] slotsInPanel = inventoryPanel.GetComponentsInChildren<InventorySlotUI>(true);
            if (slotsInPanel != null && slotsInPanel.Length > 0)
            {
                allSlots = slotsInPanel;
                return;
            }
        }

        InventorySlotUI[] slotsInScene = FindObjectsOfType<InventorySlotUI>(true);
        if (slotsInScene != null && slotsInScene.Length > 0)
        {
            allSlots = slotsInScene;
        }
    }

    private void NormalizeSlotState(InventorySlotUI slot)
    {
        if (slot == null) return;

        if (slot.currentItemAsset == null || slot.CurrentCount <= 0)
        {
            slot.ClearSlot();
            return;
        }

        if (!slot.isFull)
        {
            slot.SetItem(slot.currentItemAsset, slot.CurrentCount);
        }
    }

    private bool IsSameInventoryItem(ItemAsset first, ItemAsset second)
    {
        if (first == null || second == null)
        {
            return false;
        }

        if (first == second)
        {
            return true;
        }

        string firstId = GetItemSaveId(first);
        string secondId = GetItemSaveId(second);
        if (!string.IsNullOrWhiteSpace(firstId) && !string.IsNullOrWhiteSpace(secondId))
        {
            return firstId == secondId;
        }

        return first.name == second.name;
    }

    private string BuildSlotDebugSummary()
    {
        if (allSlots == null)
        {
            return "当前 allSlots 为 null。";
        }

        int nullSlots = 0;
        int emptySlots = 0;
        int occupiedSlots = 0;

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null)
            {
                nullSlots++;
                continue;
            }

            if (slot.HasItem)
            {
                occupiedSlots++;
            }
            else
            {
                emptySlots++;
            }
        }

        return $"格子总数：{allSlots.Length}，空格：{emptySlots}，已有物品格：{occupiedSlots}，空引用格：{nullSlots}。";
    }

    public List<InventorySaveItem> CreateInventorySaveData()
    {
        List<InventorySaveItem> result = new List<InventorySaveItem>();

        if (allSlots == null)
        {
            return result;
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null || !slot.isFull || slot.currentItemAsset == null)
            {
                continue;
            }

            string itemId = GetItemSaveId(slot.currentItemAsset);
            if (string.IsNullOrWhiteSpace(itemId))
            {
                Debug.LogWarning("Save skipped an item with empty itemId: " + slot.currentItemAsset.name);
                continue;
            }

            result.Add(new InventorySaveItem
            {
                itemId = itemId,
                amount = slot.CurrentCount
            });
        }

        return result;
    }

    public void LoadInventory(List<InventorySaveItem> savedItems)
    {
        ClearInventory();

        if (savedItems == null)
        {
            return;
        }

        foreach (InventorySaveItem savedItem in savedItems)
        {
            if (savedItem == null || savedItem.amount <= 0)
            {
                continue;
            }

            ItemAsset asset = FindItemById(savedItem.itemId);
            if (asset == null)
            {
                Debug.LogWarning("Load skipped missing itemId: " + savedItem.itemId);
                continue;
            }

            AddItem(asset, savedItem.amount);
        }
    }

    public void ClearInventory()
    {
        if (allSlots == null || allSlots.Length == 0)
        {
            if (gridParent != null)
            {
                allSlots = gridParent.GetComponentsInChildren<InventorySlotUI>(true);
            }
        }

        if (allSlots == null)
        {
            return;
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }
    }

    private ItemAsset FindItemById(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return null;
        }

        ItemAsset[] allItems = Resources.LoadAll<ItemAsset>("");

        foreach (ItemAsset item in allItems)
        {
            if (item == null)
            {
                continue;
            }

            if (item.itemId == itemId || item.name == itemId)
            {
                return item;
            }
        }

        return null;
    }

    private string GetItemSaveId(ItemAsset asset)
    {
        if (asset == null)
        {
            return string.Empty;
        }

        return string.IsNullOrWhiteSpace(asset.itemId) ? asset.name : asset.itemId;
    }

    public void OnSlotClicked(InventorySlotUI slot)
    {
        if (inventoryPanel == null || !inventoryPanel.activeSelf)
        {
            return;
        }

        if (slot == null || !slot.isFull || slot.currentItemAsset == null)
        {
            HideItemActionPanel();
            return;
        }

        EnsureItemActionPanel();

        if (selectedSlot == slot && itemActionPanel != null && itemActionPanel.activeSelf)
        {
            HideItemActionPanel();
            return;
        }

        selectedSlot = slot;
        RefreshItemActionPanel();
        PositionItemActionPanel(slot);
        if (itemActionBlocker != null)
        {
            itemActionBlocker.SetActive(true);
            itemActionBlocker.transform.SetAsLastSibling();
        }

        itemActionPanel.transform.SetAsLastSibling();
        itemActionPanel.SetActive(true);
    }

    private void EnsureItemActionPanel()
    {
        if (itemActionPanel != null || inventoryPanel == null) return;

        TMP_FontAsset fontAsset = ResolveUIFont();

        itemActionBlocker = new GameObject("ItemActionPanelBlocker", typeof(RectTransform), typeof(Image), typeof(Button));
        itemActionBlocker.layer = inventoryPanel.layer;
        itemActionBlocker.transform.SetParent(inventoryPanel.transform, false);

        RectTransform blockerRect = itemActionBlocker.GetComponent<RectTransform>();
        blockerRect.anchorMin = Vector2.zero;
        blockerRect.anchorMax = Vector2.one;
        blockerRect.offsetMin = Vector2.zero;
        blockerRect.offsetMax = Vector2.zero;
        blockerRect.localScale = Vector3.one;
        blockerRect.localRotation = Quaternion.identity;

        Image blockerImage = itemActionBlocker.GetComponent<Image>();
        blockerImage.sprite = null;
        blockerImage.type = Image.Type.Simple;
        blockerImage.color = new Color(0f, 0f, 0f, 0f);
        blockerImage.raycastTarget = true;

        Button blockerButton = itemActionBlocker.GetComponent<Button>();
        blockerButton.targetGraphic = blockerImage;
        blockerButton.transition = Selectable.Transition.None;
        blockerButton.onClick.AddListener(HideItemActionPanel);
        itemActionBlocker.SetActive(false);

        itemActionPanel = new GameObject("ItemActionPanel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        itemActionPanel.layer = inventoryPanel.layer;
        itemActionPanel.transform.SetParent(inventoryPanel.transform, false);

        itemActionPanelRect = itemActionPanel.GetComponent<RectTransform>();
        itemActionPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemActionPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemActionPanelRect.pivot = new Vector2(0f, 1f);
        itemActionPanelRect.sizeDelta = new Vector2(240f, 118f);

        Image panelImage = itemActionPanel.GetComponent<Image>();
        panelImage.sprite = null;
        panelImage.type = Image.Type.Simple;
        panelImage.color = panelColor;

        itemNameText = CreateText("ItemName", itemActionPanel.transform, fontAsset, 28f, FontStyles.Bold, TextAlignmentOptions.Center);
        SetRect(itemNameText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -12f), new Vector2(208f, 32f), new Vector2(0f, 1f));

        itemCountText = CreateText("ItemCount", itemActionPanel.transform, fontAsset, 20f, FontStyles.Normal, TextAlignmentOptions.Center);
        itemCountText.color = new Color(0.82f, 0.86f, 0.92f, 1f);
        SetRect(itemCountText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(16f, -44f), new Vector2(208f, 24f), new Vector2(0f, 1f));

        useButton = CreateActionButton(itemActionPanel.transform, "UseButton", "\u4F7F\u7528", new Vector2(16f, -76f), fontAsset, out useButtonLabel, OnClickUseButton);
        dropButton = CreateActionButton(itemActionPanel.transform, "DropButton", "\u4E22\u5F03", new Vector2(124f, -76f), fontAsset, out dropButtonLabel, OnClickDropButton);
    }

    private void RefreshItemActionPanel()
    {
        if (selectedSlot == null || !selectedSlot.isFull || selectedSlot.currentItemAsset == null)
        {
            HideItemActionPanel();
            return;
        }

        ItemAsset asset = selectedSlot.currentItemAsset;
        itemNameText.text = asset.itemName;
        itemCountText.text = $"\u6570\u91CF\uFF1A{selectedSlot.CurrentCount}";

        Image useButtonImage = useButton.GetComponent<Image>();
        if (useButtonImage != null)
        {
            useButtonImage.color = buttonColor;
        }

        if (useButtonLabel != null)
        {
            useButtonLabel.color = Color.black;
        }

        Image dropButtonImage = dropButton.GetComponent<Image>();
        if (dropButtonImage != null)
        {
            dropButtonImage.color = buttonColor;
        }

        if (dropButtonLabel != null)
        {
            dropButtonLabel.color = Color.black;
        }
    }

    private void PositionItemActionPanel(InventorySlotUI slot)
    {
        if (itemActionPanelRect == null || inventoryPanel == null || slot == null) return;

        RectTransform inventoryRect = inventoryPanel.transform as RectTransform;
        RectTransform slotRect = slot.transform as RectTransform;
        if (inventoryRect == null || slotRect == null) return;

        Vector3[] corners = new Vector3[4];
        slotRect.GetWorldCorners(corners);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRect,
            RectTransformUtility.WorldToScreenPoint(null, corners[3]),
            null,
            out Vector2 localPoint);

        Vector2 desiredPosition = localPoint + new Vector2(-20f, 20f);
        float panelWidth = itemActionPanelRect.sizeDelta.x;
        float panelHeight = itemActionPanelRect.sizeDelta.y;

        float minX = inventoryRect.rect.xMin;
        float maxX = inventoryRect.rect.xMax - panelWidth;
        float minY = inventoryRect.rect.yMin + panelHeight;
        float maxY = inventoryRect.rect.yMax;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

        itemActionPanelRect.anchoredPosition = desiredPosition;
    }

    private void OnClickUseButton()
    {
        if (selectedSlot == null || selectedSlot.currentItemAsset == null) return;

        string itemName = selectedSlot.currentItemAsset.itemName;
        if (selectedSlot.currentItemAsset.bagObject != null)
        {
            selectedSlot.currentItemAsset.bagObject.Use();
        }
        RemoveItemsFromSlot(selectedSlot, 1);
        Debug.Log($"使用了 1 个 {itemName}。");
        RefreshOrHideActionPanel();
    }

    private void OnClickDropButton()
    {
        if (selectedSlot == null || selectedSlot.currentItemAsset == null) return;

        string itemName = selectedSlot.currentItemAsset.itemName;
        RemoveItemsFromSlot(selectedSlot, 1);
        Debug.Log($"丢弃了 1 个 {itemName}。");
        RefreshOrHideActionPanel();
    }

    private void RefreshOrHideActionPanel()
    {
        if (selectedSlot == null || !selectedSlot.isFull || selectedSlot.currentItemAsset == null)
        {
            HideItemActionPanel();
            return;
        }

        RefreshItemActionPanel();
    }

    public void RemoveItemsFromSlot(InventorySlotUI slot, int amount)
    {
        if (slot == null || amount <= 0) return;

        int nextCount = slot.CurrentCount - amount;
        if (nextCount > 0)
        {
            slot.SetItem(slot.currentItemAsset, nextCount);
        }
        else
        {
            slot.ClearSlot();
        }
    }

    private void HideItemActionPanel()
    {
        selectedSlot = null;

        if (itemActionPanel != null)
        {
            itemActionPanel.SetActive(false);
        }

        if (itemActionBlocker != null)
        {
            itemActionBlocker.SetActive(false);
        }
    }

    private TMP_FontAsset ResolveUIFont()
    {
        if (TooltipManager.Instance != null && TooltipManager.Instance.nameText != null)
        {
            return TooltipManager.Instance.nameText.font;
        }

        if (allSlots != null)
        {
            foreach (InventorySlotUI slot in allSlots)
            {
                if (slot != null && slot.amountText != null)
                {
                    return slot.amountText.font;
                }
            }
        }

        return TMP_Settings.defaultFontAsset;
    }

    private TextMeshProUGUI CreateText(string objectName, Transform parent, TMP_FontAsset fontAsset, float fontSize, FontStyles fontStyle, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform));
        textObject.layer = inventoryPanel.layer;
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.font = fontAsset;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private Button CreateActionButton(Transform parent, string objectName, string label, Vector2 anchoredPosition, TMP_FontAsset fontAsset, out TextMeshProUGUI labelText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.layer = inventoryPanel.layer;
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        SetRect(buttonRect, new Vector2(0f, 1f), new Vector2(0f, 1f), anchoredPosition, new Vector2(100f, 34f), new Vector2(0f, 1f));

        Image image = buttonObject.GetComponent<Image>();
        image.sprite = null;
        image.type = Image.Type.Simple;
        image.color = buttonColor;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        labelText = CreateText("Label", buttonObject.transform, fontAsset, 20f, FontStyles.Normal, TextAlignmentOptions.Center);
        labelText.text = label;
        labelText.color = Color.black;
        SetRect(labelText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

        return button;
    }

    private void SetRect(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Vector2 pivot)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = pivot;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }
}
