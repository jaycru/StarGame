using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "LogicCat/Item")]
public class ItemAsset : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;
}
