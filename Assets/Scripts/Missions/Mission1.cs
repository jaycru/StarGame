using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：任务1：杀死一个怪物
//*************************

public class Mission1 : MonoBehaviour
{
    public GameObject enemy1; // 任务目标怪物
    public QuestData questData; // 任务数据
    private GameObject currentEnemyInstance; // 当前实例化的敌人
    void Start()
    {
        currentEnemyInstance = Instantiate(enemy1);
    }

    void Update()
    {
        if (currentEnemyInstance == null)
        {
            Debug.Log("Mission1: Enemy defeated!"); // 打印日志
            QuestManager.Instance.CompleteQuest(questData.questId, true); // 完成任务
            Destroy(gameObject); // 销毁任务脚本
        }
    }
}
