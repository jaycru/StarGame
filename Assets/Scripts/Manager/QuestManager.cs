using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：对话管理器，控制中枢
//*************************

public class QuestManager : MonoBehaviour
{
    public GameObject missionNameText;//任务名称面板
    public static QuestManager Instance { get; private set; }
    [SerializeField]
    private List<QuestData> allQuests; // 所有对话数据列表

    private Dictionary<string, QuestState> questStates = new(); // 对话状态字典，key为对话ID

    public event Action<string, QuestState> OnQuestStateChanged; // 对话状态改变事件

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        foreach (var quest in allQuests)
        {
            //无前置的任务设置为可接受，有前置的任务设置为不可接受
            if (quest.preQuestIds == null || quest.preQuestIds.Length == 0)
            {
                questStates[quest.questId] = QuestState.Available;
            }
            else
            {
                questStates[quest.questId] = QuestState.Locked;
            }
        }
    }
    /// <summary>
    /// 获取对话当前状态
    /// </summary>
    /// <param name="questId"></param>
    /// <returns></returns>
    public QuestState GetQuestState(string questId)
    {
        return questStates.TryGetValue(questId, out var state) ? state : QuestState.Locked;
    }
    /// <summary>
    /// 接受对话
    /// </summary>
    /// <param name="questId"></param>
    /// <returns></returns>
    public bool AcceptQuest(string questId, QuestData missionQuest = null)
    {
        Debug.Log("AcceptQuest: " + questId);
        Debug.Log("Current State:" + GetQuestState(questId));
        if (GetQuestState(questId) != QuestState.Available)
            return false;

        questStates[questId] = QuestState.Active;
        OnQuestStateChanged?.Invoke(questId, QuestState.Active);

        if (missionQuest != null)
        {
            missionNameText.GetComponent<TextMeshProUGUI>().text = missionQuest.questName;
        }
        return true;
    }
    /// <summary>
    /// 结束对话，并解锁后续任务
    /// </summary>
    /// <param name="questId"></param>
    /// <param name="needReset">是否需要显示当前无任务</param>
    /// <returns></returns>
    public bool CompleteQuest(string questId, bool needReset = false)
    {
        Debug.Log("CompleteQuest: " + questId);
        Debug.Log("Current State:" + GetQuestState(questId));
        if (GetQuestState(questId) != QuestState.Active)
            return false;

        questStates[questId] = QuestState.Completed;
        OnQuestStateChanged?.Invoke(questId, QuestState.Completed);
        UnlockNextQuests(questId);

        if (needReset)
        {
            missionNameText.GetComponent<TextMeshProUGUI>().text = "当前无任务";
        }

        return true;
    }
    /// <summary>
    /// 解锁后续任务
    /// </summary>
    /// <param name="questId"></param>
    private void UnlockNextQuests(string questId)
    {
        foreach (var quest in allQuests)
        {
            Debug.Log("Checking quest: " + quest.questId);
            if (quest.preQuestIds == null)
            {
                continue;
            }
            foreach (var preId in quest.preQuestIds)
            {
                if (preId == questId && CanUnlock(quest))
                {
                    questStates[quest.questId] = QuestState.Available;
                    OnQuestStateChanged?.Invoke(quest.questId, QuestState.Available);
                }
            }
        }
    }
    /// <summary>
    /// 判断任务是否可以解锁
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    private bool CanUnlock(QuestData quest)
    {
        foreach (var preId in quest.preQuestIds)
        {
            if (GetQuestState(preId) != QuestState.Completed)
            {
                return false;
            }
        }
        return true;
    }
}
