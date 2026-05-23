using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务数据。
/// 创建方式：右键 Project 面板 -> Create -> ARScanGame -> TaskData。
/// </summary>
[CreateAssetMenu(fileName = "TaskData", menuName = "ARScanGame/TaskData")]
public class TaskData : ScriptableObject
{
    [Header("任务信息")]
    public string taskId;
    public string taskName;
    [TextArea]
    public string description;

    [Header("任务目标")]
    public List<ItemData> requiredItems = new List<ItemData>();

    [Header("奖励信息")]
    public string rewardName;
    [TextArea]
    public string rewardDescription;

    public int GetTotalItemCount()
    {
        if (requiredItems == null)
        {
            return 0;
        }

        return requiredItems.Count;
    }

    public bool ContainsItem(ItemData itemData)
    {
        if (itemData == null || requiredItems == null)
        {
            return false;
        }

        for (int i = 0; i < requiredItems.Count; i++)
        {
            if (requiredItems[i] != null && requiredItems[i].itemId == itemData.itemId)
            {
                return true;
            }
        }

        return false;
    }
}
