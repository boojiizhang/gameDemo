using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务 UI。
/// 挂载位置：ARScanScene / Canvas_AR / TaskUI。
/// </summary>
public class TaskUI : MonoBehaviour
{
    [Header("文本组件")]
    public Text taskNameText;
    public Text taskDescText;
    public Text progressText;

    private void OnEnable()
    {
        GameEvents.OnTaskAccepted += RefreshTask;
        GameEvents.OnTaskProgressChanged += RefreshProgress;
        RefreshFromManager();
    }

    private void OnDisable()
    {
        GameEvents.OnTaskAccepted -= RefreshTask;
        GameEvents.OnTaskProgressChanged -= RefreshProgress;
    }

    private void RefreshFromManager()
    {
        if (TaskManager.Instance != null && TaskManager.Instance.HasAcceptedTask())
        {
            RefreshTask(TaskManager.Instance.GetCurrentTask());
            RefreshProgress(TaskManager.Instance.GetCurrentProgress(), TaskManager.Instance.GetTotalProgress());
        }
        else
        {
            SetText(taskNameText, "尚未领取任务");
            SetText(taskDescText, "请先扫描任务目标");
            SetText(progressText, "进度：0/0");
        }
    }

    private void RefreshTask(TaskData taskData)
    {
        if (taskData == null)
        {
            return;
        }

        SetText(taskNameText, taskData.taskName);
        SetText(taskDescText, taskData.description);
    }

    private void RefreshProgress(int current, int total)
    {
        SetText(progressText, "进度：" + current + "/" + total);
    }

    private void SetText(Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value;
        }
    }
}
