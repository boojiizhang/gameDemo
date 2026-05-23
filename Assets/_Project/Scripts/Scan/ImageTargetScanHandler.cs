using UnityEngine;

#if VUFORIA_PRESENT
using Vuforia;
#endif

/// <summary>
/// 单个 ImageTarget 的识别监听脚本。
/// 挂载位置：每个 Vuforia ImageTarget 物体。
///
/// 使用真实 Vuforia 时：
/// 1. 先导入 Vuforia Engine。
/// 2. 在 Player Settings -> Scripting Define Symbols 添加 VUFORIA_PRESENT。
/// 3. 把本脚本挂到 ImageTarget 上。
/// </summary>
public class ImageTargetScanHandler : MonoBehaviour
{
    [Header("扫描目标配置")]
    public ScanTargetType targetType = ScanTargetType.Item;
    public string targetId;
    public TaskData taskData;
    public ItemData itemData;

    [Header("识别成功后显示的 AR 内容")]
    public GameObject successVisual;

    private bool hasReported;

#if VUFORIA_PRESENT
    private ObserverBehaviour observerBehaviour;

    private void Awake()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
    }

    private void OnEnable()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }

        SetSuccessVisualActive(false);
    }

    private void OnDisable()
    {
        if (observerBehaviour != null)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (IsTracked(targetStatus))
        {
            NotifyScanSuccess();
        }
        else
        {
            SetSuccessVisualActive(false);
        }
    }

    private bool IsTracked(TargetStatus targetStatus)
    {
        return targetStatus.Status == Status.TRACKED ||
               targetStatus.Status == Status.EXTENDED_TRACKED ||
               targetStatus.Status == Status.LIMITED;
    }
#else
    private void OnEnable()
    {
        SetSuccessVisualActive(false);
    }
#endif

    /// <summary>
    /// 测试用：未接入 Vuforia 时，可以在 Inspector 右键菜单里模拟扫描成功。
    /// </summary>
    [ContextMenu("模拟扫描成功")]
    public void SimulateScanForTest()
    {
        NotifyScanSuccess();
    }

    private void NotifyScanSuccess()
    {
        if (hasReported)
        {
            return;
        }

        hasReported = true;
        SetSuccessVisualActive(true);

        if (ScanManager.Instance != null)
        {
            ScanManager.Instance.HandleTargetScanned(this);
        }
        else
        {
            Debug.LogWarning("[ImageTargetScanHandler] 场景中没有 ScanManager，无法处理扫描结果。");
        }
    }

    private void SetSuccessVisualActive(bool active)
    {
        if (successVisual != null)
        {
            successVisual.SetActive(active);
        }
    }
}
