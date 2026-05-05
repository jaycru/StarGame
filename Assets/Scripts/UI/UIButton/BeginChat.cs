using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：开始对话的UI按钮
//***************************************** 
public class BeginChat : HoldButton
{
    private QuestData questData;//对话任务
    private TextAsset textAsset;
    private Transform chatTextTrans;
    private TextOut textOut;
    protected override void Effect()
    {
        GameSceneManager.instance.SetGamePlayActive(true, textAsset, questData);
        gameObject.SetActive(false);
    }

    public void SetText(TextAsset textAsset)
    {
        this.textAsset = textAsset;
    }

    public void SetQuestData(QuestData questData)
    {
        this.questData = questData;
    }
}
