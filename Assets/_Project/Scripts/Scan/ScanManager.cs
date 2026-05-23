using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 扫描系统管理器。
/// 挂载位置：ARScanScene / ScanManager。
/// </summary>
public class ScanManager : MonoBehaviour
{
    public static ScanManager Instance;

    [Header("扫描成功 UI")]
    public ScanSuccessUI scanSuccessUI;

    [Header("提示文字")]
    public string scanTaskFirstMessage = "请先扫描任务目标领取任务";
    public string duplicateMessage = "这个目标已经扫描过了";
    public string invalidTargetMessage = "扫描目标配置不完整";

    private readonly List<string> scannedTargetIds = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// ImageTargetScanHandler 识别成功后会调用这个方法。
    /// </summary>
    public void HandleTargetScanned(ImageTargetScanHandler handler)
    {
        if (handler == null)
        {
            ShowMessage(invalidTargetMessage);
            return;
        }

        if (string.IsNullOrEmpty(handler.targetId))
        {
            ShowMessage(invalidTargetMessage);
            return;
        }

        if (handler.targetType == ScanTargetType.Task)
        {
            HandleTaskScanned(handler.targetId, handler.taskData);
            return;
        }

        HandleItemScanned(handler.targetId, handler.itemData);
    }

    /// <summary>
    /// 测试用：不接 Vuforia 时，可以在按钮里调用这个方法模拟领取任务。
    /// </summary>
    public void SimulateTaskScan(TaskData taskData)
    {
        string targetId = "task_simulate";
        if (taskData != null && !string.IsNullOrEmpty(taskData.taskId))
        {
            targetId = taskData.taskId;
        }

        HandleTaskScanned(targetId, taskData);
    }

    /// <summary>
    /// 测试用：不接 Vuforia 时，可以在按钮里调用这个方法模拟获得道具。
    /// </summary>
    public void SimulateItemScan(ItemData itemData)
    {
        string targetId = "item_simulate";
        if (itemData != null && !string.IsNullOrEmpty(itemData.targetId))
        {
            targetId = itemData.targetId;
        }

        HandleItemScanned(targetId, itemData);
    }

    private void HandleTaskScanned(string targetId, TaskData taskData)
    {
        if (taskData == null)
        {
            ShowMessage(invalidTargetMessage);
            return;
        }

        if (HasScanned(targetId))
        {
            ShowMessage(duplicateMessage);
            return;
        }

        scannedTargetIds.Add(targetId);
        GameEvents.RaiseTaskScanned(taskData);
        ShowMessage("任务已领取：" + taskData.taskName);
    }

    private void HandleItemScanned(string targetId, ItemData itemData)
    {
        if (itemData == null)
        {
            ShowMessage(invalidTargetMessage);
            return;
        }

        if (TaskManager.Instance == null || !TaskManager.Instance.HasAcceptedTask())
        {
            ShowMessage(scanTaskFirstMessage);
            return;
        }

        if (!TaskManager.Instance.IsRequiredItem(itemData))
        {
            ShowMessage("这个道具不属于当前任务：" + itemData.itemName);
            return;
        }

        if (HasScanned(targetId))
        {
            ShowMessage(duplicateMessage);
            return;
        }

        scannedTargetIds.Add(targetId);
        GameEvents.RaiseItemScanned(itemData);
        ShowMessage("获得道具：" + itemData.itemName);
    }

    private bool HasScanned(string targetId)
    {
        for (int i = 0; i < scannedTargetIds.Count; i++)
        {
            if (scannedTargetIds[i] == targetId)
            {
                return true;
            }
        }

        return false;
    }

    private void ShowMessage(string message)
    {
        if (scanSuccessUI != null)
        {
            scanSuccessUI.Show(message);
        }

        Debug.Log("[ScanManager] " + message);
    }
}
