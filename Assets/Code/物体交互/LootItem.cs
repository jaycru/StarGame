using UnityEngine;

[System.Serializable]
public class LootItem
{
    public ItemAsset details; // 关联我们创建的 ScriptableObject 身份卡
    public int amount;        // 数量
}
