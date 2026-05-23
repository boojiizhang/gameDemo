using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏总流程管理器。
/// 挂载位置：GameManagers.prefab / GameManager。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("场景名称")]
    public string startSceneName = "StartScene";
    public string arScanSceneName = "ARScanScene";
    public string rewardSceneName = "RewardScene";

    [Header("流程设置")]
    public bool autoLoadRewardSceneWhenTaskCompleted = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameEvents.OnTaskCompleted += HandleTaskCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskCompleted -= HandleTaskCompleted;
    }

    private void HandleTaskCompleted(TaskData taskData)
    {
        if (autoLoadRewardSceneWhenTaskCompleted)
        {
            LoadRewardScene();
        }
    }

    public void StartGame()
    {
        ResetGame();
        LoadARScanScene();
    }

    public void LoadStartScene()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void LoadARScanScene()
    {
        SceneManager.LoadScene(arScanSceneName);
    }

    public void LoadRewardScene()
    {
        SceneManager.LoadScene(rewardSceneName);
    }

    public void ResetGame()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ResetInventory();
        }

        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.ResetTask();
        }

        if (RewardManager.Instance != null)
        {
            RewardManager.Instance.ResetReward();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
