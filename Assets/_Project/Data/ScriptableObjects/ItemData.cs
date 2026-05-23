using UnityEngine;

/// <summary>
/// 道具数据。
/// 创建方式：右键 Project 面板 -> Create -> ARScanGame -> ItemData。
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ARScanGame/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemId;
    public string itemName;
    [TextArea]
    public string description;

    [Header("显示资源")]
    public Sprite icon;

    [Header("扫描配置")]
    public string targetId;

    public bool IsSameItem(ItemData other)
    {
        if (other == null)
        {
            return false;
        }

        return itemId == other.itemId;
    }
}
