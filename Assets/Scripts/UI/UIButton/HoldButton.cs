using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//*****************************************
//创建人： Jaycr 
//功能说明：控制UI界面的按钮（父类）
//***************************************** 
public class HoldButton : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    private Color normalColor = new Color32(240, 240, 240, 45);
    private Color processedColor = new Color32(0, 0, 0, 158);
    private Image buttonImage;
    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonImage.color = normalColor;
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("按钮保持按下状态");
            StartDown();
        }
        else if (Input.GetButtonUp("Interact"))
        {
            EndDown();
        }
    } 
    /// <summary>
    /// 开始触发按钮按下
    /// </summary>
    private void StartDown()
    {
        buttonImage.color = processedColor;
    }
    /// <summary>
    /// 结束按钮按下并触发按钮效果
    /// </summary>
    private void EndDown()
    {
        buttonImage.color = normalColor;
        Effect();
        Debug.Log("按钮退出，触发对应效果");
    }

    protected virtual void Effect()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartDown();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndDown();
    }
}
