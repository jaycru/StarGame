using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：挂载于NPC上，当主角靠近时打开AskPanel
//***************************************** 
public class Chat : MonoBehaviour
{
    public TextAsset initialChatText;//初始对话文本（闲聊）
    public QuestData questData;//任务对话
    private string player = "Player";
    private Transform askPanel;//对话按钮

    private void Start()
    {
        askPanel = GameObject.Find("InventoryCanvas").transform.GetChild(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == player)
        {
            askPanel.gameObject.SetActive(true);
            askPanel.GetChild(0).gameObject.SetActive(true);
            GainChatText();
            askPanel.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"(F){gameObject.name}";
            Debug.Log("靠近！");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (askPanel.gameObject.activeSelf)
        {
            askPanel.gameObject.SetActive(false);
            Debug.Log("退出！");
        }
    }
    /// <summary>
    /// 拿到对话文本
    /// </summary>
    private void GainChatText()
    {
        Debug.Log(questData.questId + " : " + QuestManager.Instance.GetQuestState(questData.questId));
        if (QuestManager.Instance.GetQuestState(questData.questId) == QuestState.Available)
        {
            askPanel.GetChild(0).GetComponent<BeginChat>().SetText(questData.dialogueText);
            askPanel.GetChild(0).GetComponent<BeginChat>().SetQuestData(questData);
        }
        else
        {
            askPanel.GetChild(0).GetComponent<BeginChat>().SetText(initialChatText);
            askPanel.GetChild(0).GetComponent<BeginChat>().SetQuestData(null);
        }
    }
}
