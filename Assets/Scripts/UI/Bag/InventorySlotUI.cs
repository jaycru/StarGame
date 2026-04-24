using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // 必须引用，用于处理鼠标进入和退出事件
//*****************************************
//创建人： yzh
//功能说明：该代码和背包格子，也就是预制体InventorySlot链接
//***************************************** 
public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 组件引用")]
    public Image iconImage;             // 格子里的物品图标
    public TextMeshProUGUI amountText;  // 格子里的数量文字

    [Header("当前格子状态")]
    public bool isFull = false;         // 标记该格子是否被占用
    public ItemAsset currentItemAsset;  // 存储当前格子的物品数据资产

    void Awake()
    {
        RefreshVisual();
    }

    /// <summary>
    /// 向格子里放入物品并刷新显示
    /// </summary>
    public void SetItem(ItemAsset asset, int count)
    {
        currentItemAsset = asset;
        isFull = true;

        RefreshVisual(count);
    }

    private void RefreshVisual(int count = 0)
    {
        if (!isFull || currentItemAsset == null)
        {
            ClearVisual();
            return;
        }

        // 设置图标
        if (iconImage != null)
        {
            iconImage.sprite = currentItemAsset.icon;
            // 如果资产里没有图片，就把 Image 组件关掉，防止显示白方块
            iconImage.enabled = (currentItemAsset.icon != null);
        }

        // 设置数量文字
        if (amountText != null)
        {
            if (count > 0)
            {
                amountText.text = count.ToString();
            }
            amountText.enabled = true;
        }
    }

    /// <summary>
    /// 清空该格子
    /// </summary>
    public void ClearSlot()
    {
        currentItemAsset = null;
        isFull = false;

        ClearVisual();
    }

    private void ClearVisual()
    {
        if (iconImage != null) iconImage.enabled = false;
        if (amountText != null) amountText.enabled = false;
    }

    // --- 鼠标交互逻辑 (EventSystem 接口实现) ---

    // 鼠标移入格子瞬间触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 只有格子里有东西时，才呼叫提示框管理器
        if (isFull && currentItemAsset != null)
        {
            if (TooltipManager.Instance != null)
            {
                TooltipManager.Instance.ShowTooltip(currentItemAsset.itemName, currentItemAsset.description);
            }
        }
    }

    // 鼠标移出格子瞬间触发
    public void OnPointerExit(PointerEventData eventData)
    {
        // 告诉提示框管理器隐藏窗口
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }
}

