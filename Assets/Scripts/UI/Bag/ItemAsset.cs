using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "LogicCat/Item")]
public class ItemAsset : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public BagObject bagObject;//对应的背包物体代码
    [TextArea] public string description; // 物品描述
}
