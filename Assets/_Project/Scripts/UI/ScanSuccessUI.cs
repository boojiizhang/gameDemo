using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 扫描成功提示 UI。
/// 挂载位置：ScanSuccessUI.prefab。
/// </summary>
public class ScanSuccessUI : MonoBehaviour
{
    [Header("UI 组件")]
    public CanvasGroup canvasGroup;
    public RectTransform panelTransform;
    public Text messageText;

    [Header("动画设置")]
    public float showTime = 1.2f;
    public float fadeTime = 0.2f;

    private Coroutine showCoroutine;

    private void Awake()
    {
        HideImmediately();
    }

    public void Show(string message)
    {
        // 先激活物体，再启动协程；否则隐藏状态下无法启动协程。
        SetVisible(true);

        if (messageText != null)
        {
            messageText.text = message;
        }

        if (showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }

        showCoroutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            SetAlpha(t);
            SetScale(0.85f + 0.15f * t);
            yield return null;
        }

        SetAlpha(1f);
        SetScale(1f);
        yield return new WaitForSeconds(showTime);

        timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            SetAlpha(1f - t);
            yield return null;
        }

        HideImmediately();
    }

    private void HideImmediately()
    {
        SetAlpha(0f);
        SetScale(0.85f);
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }

    private void SetScale(float scale)
    {
        if (panelTransform != null)
        {
            panelTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}
