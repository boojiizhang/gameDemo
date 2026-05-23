using UnityEngine;

/// <summary>
/// 简单 UI 管理器。
/// 挂载位置：每个场景的 Canvas 或 UIManager 物体。
/// </summary>
public class UIManager : MonoBehaviour
{
    public void Show(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    public void Hide(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void SwitchPanel(GameObject showPanel, GameObject hidePanel)
    {
        Show(showPanel);
        Hide(hidePanel);
    }
}
