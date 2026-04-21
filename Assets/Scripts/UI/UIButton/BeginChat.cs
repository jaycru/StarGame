using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：开始对话的UI按钮
//***************************************** 
public class BeginChat : HoldButton
{
    private TextAsset textAsset;
    private Transform chatTextTrans;
    private TextOut textOut;
    protected override void Effect()
    {
        Time.timeScale = 0;
        chatTextTrans = transform.parent.GetChild(1);//初始化文本UI块
        textOut = chatTextTrans.GetComponent<TextOut>();//初始化对应TextOut脚本
        textOut.SetText(textAsset);
        chatTextTrans.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("开始对话");
        gameObject.SetActive(false);
    }

    public void SetText(TextAsset textAsset)
    {
        this.textAsset = textAsset;
    }
}
