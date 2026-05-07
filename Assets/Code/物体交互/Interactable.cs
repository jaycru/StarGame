using UnityEngine;
using System.Collections.Generic;

// 定义交互类型：简单动作(门) 或 拾取清单(物资)
public enum InteractType { Simple, LootList }

public class Interactable : MonoBehaviour
{
    [Header("存档")]
    public string saveId;

    public InteractType type; // 在 Inspector 里选类型
    public string simpleActionName = "开门"; // 如果是门，写这

    [Header("如果是物资，填这里")]
    public List<LootItem> itemsInObject = new List<LootItem>();

    public void DoAction()
    {
        // 这里以后写开门动画、捡东西逻辑
        Debug.Log("触发了: " + simpleActionName);
    }

    public string GetSaveId()
    {
        if (!string.IsNullOrWhiteSpace(saveId))
        {
            return saveId;
        }

        return gameObject.scene.name + "/" + GetHierarchyPath(transform);
    }

    private string GetHierarchyPath(Transform target)
    {
        if (target == null)
        {
            return string.Empty;
        }

        List<string> parts = new List<string>();
        Transform current = target;
        while (current != null)
        {
            parts.Add(current.name);
            current = current.parent;
        }

        parts.Reverse();
        return string.Join("/", parts);
    }
}
