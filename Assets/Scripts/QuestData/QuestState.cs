using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：运行时对话状态
//*************************

public enum QuestState
{
    Locked,    // 对话锁定，未满足前置条件
    Available, // 对话可接受，满足前置条件但未接受
    Active,    // 对话已接受，正在进行中
    Completed, // 对话已完成
}
