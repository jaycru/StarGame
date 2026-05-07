using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    [Header("State")]
    public bool isFull;
    public ItemAsset currentItemAsset;

    [SerializeField] private int currentCount;

    public int CurrentCount => currentCount;
    public bool HasItem => isFull && currentItemAsset != null && currentCount > 0;

    private void Awake()
    {
        NormalizeState();
        RefreshVisual();
    }

    public void SetItem(ItemAsset asset, int count)
    {
        if (asset == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        currentItemAsset = asset;
        currentCount = count;
        isFull = true;
        RefreshVisual();
    }

    public void ClearSlot()
    {
        currentItemAsset = null;
        currentCount = 0;
        isFull = false;
        ClearVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!HasItem) return;

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.ShowTooltip(currentItemAsset.itemName, currentItemAsset.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!HasItem) return;

        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSlotClicked(this);
        }
    }

    private void RefreshVisual()
    {
        if (!HasItem)
        {
            ClearVisual();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = currentItemAsset.icon;
            iconImage.enabled = currentItemAsset.icon != null;
        }

        if (amountText != null)
        {
            amountText.text = currentCount.ToString();
            amountText.enabled = true;
        }
    }

    private void ClearVisual()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (amountText != null)
        {
            amountText.text = string.Empty;
            amountText.enabled = false;
        }
    }

    private void NormalizeState()
    {
        if (currentItemAsset == null || currentCount <= 0)
        {
            currentItemAsset = null;
            currentCount = 0;
            isFull = false;
            return;
        }

        isFull = true;
    }
}
