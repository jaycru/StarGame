using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//*****************************************
//创建人：yzh
//功能说明：该代码挂载于背包界面的TooltipPnel，用于进行物体介绍
//***************************************** 
public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipWindow;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    private RectTransform rectTransform;

    void Awake()
    {
        Instance = this;
        rectTransform = tooltipWindow.GetComponent<RectTransform>();
    }

    void Update()
    {
        // 如果提示框开着，就让它跟着鼠标走
        if (tooltipWindow.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Vector2 mousePos = Input.mousePosition;

        // 获取 UI 缩放比例（适配 Canvas Scaler）
        float canvasScale = GetComponentInParent<Canvas>().scaleFactor;

        // 偏移量：根据你设定的 Pivot (0,1) 进行微调
        // 15 像素的偏移，防止提示框正对着鼠标尖
        rectTransform.position = mousePos + new Vector2(15f * canvasScale, -15f * canvasScale);
    }

    public void ShowTooltip(string name, string desc)
    {
        nameText.text = name;
        descText.text = desc;
        tooltipWindow.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipWindow.SetActive(false);
    }
}
