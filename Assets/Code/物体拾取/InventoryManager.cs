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
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if (gridParent != null)
        {
            allSlots = gridParent.GetComponentsInChildren<InventorySlotUI>(true);
            Debug.Log($"背包系统初始化成功：共识别到 {allSlots.Length} 个存储格。");
        }
        else
        {
            Debug.LogError("错误：请在 Inspector 面板将 GridWindow 物体拖入 Grid Parent 槽位！");
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
        if (asset == null)
        {
            Debug.LogWarning("拾取失败：物品数据为空！");
            return false;
        }

        if (count <= 0)
        {
            Debug.LogWarning($"拾取失败：{asset.itemName} 的数量无效。");
            return false;
        }

        if (allSlots == null || allSlots.Length == 0)
        {
            Debug.LogWarning("拾取失败：背包格子没有初始化！");
            return false;
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null) continue;

            if (slot.isFull && slot.currentItemAsset == asset)
            {
                slot.SetItem(asset, slot.CurrentCount + count);
                Debug.Log($"{asset.itemName} 堆叠成功。");
                return true;
            }
        }

        foreach (InventorySlotUI slot in allSlots)
        {
            if (slot == null) continue;

            if (!slot.isFull)
            {
                slot.SetItem(asset, count);
                Debug.Log($"{asset.itemName} 已放入背包。");
                return true;
            }
        }

        Debug.LogWarning("背包已满！");
        return false;
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

    private void RemoveItemsFromSlot(InventorySlotUI slot, int amount)
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
