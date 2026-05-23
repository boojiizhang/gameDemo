using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 奖励 UI。
/// 挂载位置：RewardScene / Canvas_Reward / RewardUI。
/// </summary>
public class RewardUI : MonoBehaviour
{
    [Header("文本组件")]
    public Text rewardTitleText;
    public Text rewardDescText;
    public Text resultText;

    [Header("按钮")]
    public Button exchangeButton;
    public Button backHomeButton;

    private void OnEnable()
    {
        GameEvents.OnRewardExchanged += RefreshExchangedState;

        if (exchangeButton != null)
        {
            exchangeButton.onClick.AddListener(ExchangeReward);
        }

        if (backHomeButton != null)
        {
            backHomeButton.onClick.AddListener(BackHome);
        }

        RefreshRewardInfo();
    }

    private void OnDisable()
    {
        GameEvents.OnRewardExchanged -= RefreshExchangedState;

        if (exchangeButton != null)
        {
            exchangeButton.onClick.RemoveListener(ExchangeReward);
        }

        if (backHomeButton != null)
        {
            backHomeButton.onClick.RemoveListener(BackHome);
        }
    }

    private void RefreshRewardInfo()
    {
        TaskData taskData = null;
        if (TaskManager.Instance != null)
        {
            taskData = TaskManager.Instance.GetCurrentTask();
        }

        if (taskData != null)
        {
            SetText(rewardTitleText, taskData.rewardName);
            SetText(rewardDescText, taskData.rewardDescription);
        }
        else
        {
            SetText(rewardTitleText, "奖励");
            SetText(rewardDescText, "完成任务后可以兑换奖励");
        }

        bool canExchange = RewardManager.Instance != null && RewardManager.Instance.CanExchangeReward();
        if (exchangeButton != null)
        {
            exchangeButton.interactable = canExchange;
        }

        if (RewardManager.Instance != null && RewardManager.Instance.HasExchangedReward())
        {
            RefreshExchangedState();
        }
        else
        {
            SetText(resultText, canExchange ? "可以兑换奖励" : "暂时不能兑换");
        }
    }

    private void ExchangeReward()
    {
        if (RewardManager.Instance != null)
        {
            RewardManager.Instance.ExchangeReward();
        }
    }

    private void RefreshExchangedState()
    {
        SetText(resultText, "兑换成功，游戏结束！");

        if (exchangeButton != null)
        {
            exchangeButton.interactable = false;
        }
    }

    private void BackHome()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadStartScene();
        }
    }

    private void SetText(Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value;
        }
    }
}
