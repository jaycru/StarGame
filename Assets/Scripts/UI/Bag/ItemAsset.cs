using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Jaycr 
//功能说明：该代码相当于所有物体的一个同一结构体，包括名字，图标和描述
//***************************************** 
// 这个指令让你能在 Project 窗口右键创建新的物资数据
[CreateAssetMenu(fileName = "NewItem", menuName = "LogicCat/Item")]
public class ItemAsset : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description; // 物品描述
}
