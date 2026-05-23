using System;

/// <summary>
/// 游戏事件中心。
/// 说明：所有 Manager 和 UI 通过这里通信，避免脚本之间互相强依赖。
/// </summary>
public static class GameEvents
{
    public static event Action<TaskData> OnTaskScanned;
    public static event Action<TaskData> OnTaskAccepted;
    public static event Action<ItemData> OnItemScanned;
    public static event Action<ItemData> OnItemAdded;
    public static event Action<int, int> OnTaskProgressChanged;
    public static event Action<TaskData> OnTaskCompleted;
    public static event Action OnRewardExchanged;

    public static void RaiseTaskScanned(TaskData taskData)
    {
        if (OnTaskScanned != null)
        {
            OnTaskScanned(taskData);
        }
    }

    public static void RaiseTaskAccepted(TaskData taskData)
    {
        if (OnTaskAccepted != null)
        {
            OnTaskAccepted(taskData);
        }
    }

    public static void RaiseItemScanned(ItemData itemData)
    {
        if (OnItemScanned != null)
        {
            OnItemScanned(itemData);
        }
    }

    public static void RaiseItemAdded(ItemData itemData)
    {
        if (OnItemAdded != null)
        {
            OnItemAdded(itemData);
        }
    }

    public static void RaiseTaskProgressChanged(int current, int total)
    {
        if (OnTaskProgressChanged != null)
        {
            OnTaskProgressChanged(current, total);
        }
    }

    public static void RaiseTaskCompleted(TaskData taskData)
    {
        if (OnTaskCompleted != null)
        {
            OnTaskCompleted(taskData);
        }
    }

    public static void RaiseRewardExchanged()
    {
        if (OnRewardExchanged != null)
        {
            OnRewardExchanged();
        }
    }
}
