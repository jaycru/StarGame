using UnityEngine;

// 这个指令让你能在 Project 窗口右键创建新的物资数据
[CreateAssetMenu(fileName = "NewItem", menuName = "LogicCat/Item")]
public class ItemAsset : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description; // 物品描述
}
