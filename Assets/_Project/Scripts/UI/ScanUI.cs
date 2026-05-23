using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扫描界面 UI。
/// 挂载位置：ARScanScene / Canvas_AR / ScanUI。
/// </summary>
public class ScanUI : MonoBehaviour
{
    [Header("文本组件")]
    public Text scanHintText;
    public Text scanStateText;

    private void OnEnable()
    {
        GameEvents.OnTaskAccepted += HandleTaskAccepted;
        GameEvents.OnItemAdded += HandleItemAdded;
        GameEvents.OnTaskCompleted += HandleTaskCompleted;

        SetText(scanHintText, "请对准任务图片开始扫描");
        SetText(scanStateText, "等待扫描");
    }

    private void OnDisable()
    {
        GameEvents.OnTaskAccepted -= HandleTaskAccepted;
        GameEvents.OnItemAdded -= HandleItemAdded;
        GameEvents.OnTaskCompleted -= HandleTaskCompleted;
    }

    private void HandleTaskAccepted(TaskData taskData)
    {
        SetText(scanHintText, "请继续扫描任务需要的道具图片");
        SetText(scanStateText, "任务已领取");
    }

    private void HandleItemAdded(ItemData itemData)
    {
        if (itemData != null)
        {
            SetText(scanStateText, "获得：" + itemData.itemName);
        }
    }

    private void HandleTaskCompleted(TaskData taskData)
    {
        SetText(scanHintText, "全部道具已收集完成");
        SetText(scanStateText, "任务完成");
    }

    private void SetText(Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value;
        }
    }
}
