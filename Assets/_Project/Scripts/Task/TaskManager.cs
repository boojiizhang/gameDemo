using UnityEngine;

/// <summary>
/// 任务管理器。
/// 挂载位置：GameManagers.prefab / TaskManager。
/// </summary>
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance;

    [Header("默认任务，可选")]
    public TaskData defaultTaskData;

    private TaskData currentTask;
    private bool taskAccepted;
    private bool taskCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnTaskScanned += AcceptTask;
        GameEvents.OnItemAdded += HandleItemAdded;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskScanned -= AcceptTask;
        GameEvents.OnItemAdded -= HandleItemAdded;
    }

    public void AcceptDefaultTask()
    {
        AcceptTask(defaultTaskData);
    }

    public void AcceptTask(TaskData taskData)
    {
        if (taskData == null)
        {
            Debug.LogWarning("[TaskManager] 任务数据为空，无法领取任务。");
            return;
        }

        if (taskAccepted && currentTask == taskData)
        {
            Debug.Log("[TaskManager] 当前任务已经领取过。");
            return;
        }

        currentTask = taskData;
        taskAccepted = true;
        taskCompleted = false;

        GameEvents.RaiseTaskAccepted(currentTask);
        RefreshProgress();
    }

    public bool HasAcceptedTask()
    {
        return taskAccepted && currentTask != null;
    }

    public bool IsTaskCompleted()
    {
        return taskCompleted;
    }

    public TaskData GetCurrentTask()
    {
        return currentTask;
    }

    public bool IsRequiredItem(ItemData itemData)
    {
        if (currentTask == null)
        {
            return false;
        }

        return currentTask.ContainsItem(itemData);
    }

    public int GetCurrentProgress()
    {
        if (currentTask == null || InventoryManager.Instance == null)
        {
            return 0;
        }

        int count = 0;
        for (int i = 0; i < currentTask.requiredItems.Count; i++)
        {
            ItemData itemData = currentTask.requiredItems[i];
            if (itemData != null && InventoryManager.Instance.HasItem(itemData))
            {
                count++;
            }
        }

        return count;
    }

    public int GetTotalProgress()
    {
        if (currentTask == null)
        {
            return 0;
        }

        return currentTask.GetTotalItemCount();
    }

    public void ResetTask()
    {
        currentTask = null;
        taskAccepted = false;
        taskCompleted = false;
        GameEvents.RaiseTaskProgressChanged(0, 0);
    }

    private void HandleItemAdded(ItemData itemData)
    {
        if (!HasAcceptedTask())
        {
            return;
        }

        RefreshProgress();
    }

    private void RefreshProgress()
    {
        int current = GetCurrentProgress();
        int total = GetTotalProgress();

        GameEvents.RaiseTaskProgressChanged(current, total);

        if (!taskCompleted && total > 0 && current >= total)
        {
            taskCompleted = true;
            GameEvents.RaiseTaskCompleted(currentTask);
        }
    }
}
