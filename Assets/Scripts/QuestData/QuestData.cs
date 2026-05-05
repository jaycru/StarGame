using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：定义每个对话（或任务）的静态数据
//*************************

[CreateAssetMenu(fileName = "Quest_", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questId;// 对话ID
    public string questName;// 任务名称（如果是任务的话）
    private string description;// 对话描述
    public TextAsset dialogueText;// 对话文本

    public string[] preQuestIds;// 前置任务ID列表

    public QuestData missionQuest;// 对话关联的任务数据
    public GameObject mission;// 任务目标物（如果是任务的话）
}
