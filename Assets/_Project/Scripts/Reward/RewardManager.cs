using UnityEngine;

/// <summary>
/// 奖励管理器。
/// 挂载位置：GameManagers.prefab / RewardManager。
/// </summary>
public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance;

    private bool rewardExchanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public bool CanExchangeReward()
    {
        if (rewardExchanged)
        {
            return false;
        }

        return TaskManager.Instance != null && TaskManager.Instance.IsTaskCompleted();
    }

    public bool HasExchangedReward()
    {
        return rewardExchanged;
    }

    public void ExchangeReward()
    {
        if (!CanExchangeReward())
        {
            Debug.Log("[RewardManager] 当前不能兑换奖励。");
            return;
        }

        rewardExchanged = true;
        GameEvents.RaiseRewardExchanged();
    }

    public void ResetReward()
    {
        rewardExchanged = false;
    }
}
