using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 项目初始化工具。
/// 用法：Unity 菜单 -> AR Scan Game -> Generate Demo Project。
/// </summary>
public static class ARScanGameSetupEditor
{
    private const string Root = "Assets/_Project";
    private const string ScenePath = Root + "/Scenes";
    private const string ItemPath = Root + "/Data/Items";
    private const string TaskPath = Root + "/Data/Tasks";
    private const string ManagerPrefabPath = Root + "/Prefabs/Managers/GameManagers.prefab";
    private const string UiPrefabPath = Root + "/Prefabs/UI";
    private const string ArPrefabPath = Root + "/Prefabs/AR";

    [MenuItem("AR Scan Game/Generate Demo Project")]
    public static void GenerateDemoProject()
    {
        EnsureFolders();

        ItemData item01 = CreateItem("Item_01", "item_01", "钥匙", "打开奖励宝箱的钥匙", "item_01_target");
        ItemData item02 = CreateItem("Item_02", "item_02", "徽章", "证明完成挑战的徽章", "item_02_target");
        ItemData item03 = CreateItem("Item_03", "item_03", "能量石", "兑换奖励需要的能量石", "item_03_target");
        TaskData task = CreateTask(item01, item02, item03);

        GameObject managerPrefab = CreateManagerPrefab(task);
        InventorySlotUI slotPrefab = CreateInventorySlotPrefab();
        ScanSuccessUI successPrefab = CreateScanSuccessPrefab();

        CreateStartScene(managerPrefab);
        CreateARScanScene(managerPrefab, task, item01, item02, item03, slotPrefab, successPrefab);
        CreateRewardScene(managerPrefab);
        UpdateBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("AR 扫描游戏示例工程已生成，三个场景已加入 Build Settings。");
    }

    private static void EnsureFolders()
    {
        CreateFolderIfMissing(Root);
        CreateFolderIfMissing(ScenePath);
        CreateFolderIfMissing(ItemPath);
        CreateFolderIfMissing(TaskPath);
        CreateFolderIfMissing(Root + "/Data/ScriptableObjects");
        CreateFolderIfMissing(Root + "/Scripts/Core");
        CreateFolderIfMissing(Root + "/Scripts/Scan");
        CreateFolderIfMissing(Root + "/Scripts/Task");
        CreateFolderIfMissing(Root + "/Scripts/Inventory");
        CreateFolderIfMissing(Root + "/Scripts/Reward");
        CreateFolderIfMissing(Root + "/Scripts/UI");
        CreateFolderIfMissing(Root + "/Prefabs/Managers");
        CreateFolderIfMissing(UiPrefabPath);
        CreateFolderIfMissing(ArPrefabPath);
        CreateFolderIfMissing(Root + "/Art/images");
        CreateFolderIfMissing(Root + "/Art/dg");
        CreateFolderIfMissing(Root + "/Art/Placeholder");
        CreateFolderIfMissing(Root + "/Settings");
    }

    private static void CreateFolderIfMissing(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string folderName = Path.GetFileName(path);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            CreateFolderIfMissing(parent);
        }

        AssetDatabase.CreateFolder(parent, folderName);
    }

    private static ItemData CreateItem(string assetName, string itemId, string itemName, string desc, string targetId)
    {
        string path = ItemPath + "/" + assetName + ".asset";
        ItemData item = AssetDatabase.LoadAssetAtPath<ItemData>(path);

        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemData>();
            AssetDatabase.CreateAsset(item, path);
        }

        item.itemId = itemId;
        item.itemName = itemName;
        item.description = desc;
        item.targetId = targetId;
        EditorUtility.SetDirty(item);
        return item;
    }

    private static TaskData CreateTask(ItemData item01, ItemData item02, ItemData item03)
    {
        string path = TaskPath + "/Task_Main.asset";
        TaskData task = AssetDatabase.LoadAssetAtPath<TaskData>(path);

        if (task == null)
        {
            task = ScriptableObject.CreateInstance<TaskData>();
            AssetDatabase.CreateAsset(task, path);
        }

        task.taskId = "task_main";
        task.taskName = "收集兑换道具";
        task.description = "先领取任务，再扫描三张道具图片。";
        task.rewardName = "比赛奖励";
        task.rewardDescription = "恭喜完成 AR 扫描收集任务！";
        task.requiredItems.Clear();
        task.requiredItems.Add(item01);
        task.requiredItems.Add(item02);
        task.requiredItems.Add(item03);
        EditorUtility.SetDirty(task);
        return task;
    }

    private static GameObject CreateManagerPrefab(TaskData task)
    {
        GameObject root = new GameObject("GameManagers");

        GameManager gameManager = root.AddComponent<GameManager>();
        gameManager.startSceneName = "StartScene";
        gameManager.arScanSceneName = "ARScanScene";
        gameManager.rewardSceneName = "RewardScene";

        TaskManager taskManager = root.AddComponent<TaskManager>();
        taskManager.defaultTaskData = task;

        root.AddComponent<InventoryManager>();
        root.AddComponent<RewardManager>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, ManagerPrefabPath);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static InventorySlotUI CreateInventorySlotPrefab()
    {
        GameObject root = CreateUIObject("InventorySlotUI");
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120f, 140f);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.35f, 0.35f, 0.35f, 1f);

        GameObject icon = CreateText("IconText", root.transform, "?", 36, TextAnchor.MiddleCenter);
        SetRect(icon, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), new Vector2(80f, 50f), Vector2.zero);

        GameObject name = CreateText("NameText", root.transform, "道具", 18, TextAnchor.MiddleCenter);
        SetRect(name, new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.34f), new Vector2(110f, 28f), Vector2.zero);

        GameObject state = CreateText("StateText", root.transform, "未获得", 16, TextAnchor.MiddleCenter);
        SetRect(state, new Vector2(0.5f, 0.14f), new Vector2(0.5f, 0.14f), new Vector2(110f, 24f), Vector2.zero);

        InventorySlotUI slot = root.AddComponent<InventorySlotUI>();
        slot.backgroundImage = bg;
        slot.iconText = icon.GetComponent<Text>();
        slot.nameText = name.GetComponent<Text>();
        slot.stateText = state.GetComponent<Text>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, UiPrefabPath + "/InventorySlotUI.prefab");
        Object.DestroyImmediate(root);
        return prefab.GetComponent<InventorySlotUI>();
    }

    private static ScanSuccessUI CreateScanSuccessPrefab()
    {
        GameObject root = CreateUIObject("ScanSuccessUI");
        CanvasGroup group = root.AddComponent<CanvasGroup>();
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420f, 120f);

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.72f);

        GameObject msg = CreateText("MessageText", root.transform, "获得道具", 28, TextAnchor.MiddleCenter);
        SetRectStretch(msg, Vector2.zero, Vector2.zero);

        ScanSuccessUI ui = root.AddComponent<ScanSuccessUI>();
        ui.canvasGroup = group;
        ui.panelTransform = rect;
        ui.messageText = msg.GetComponent<Text>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, UiPrefabPath + "/ScanSuccessUI.prefab");
        Object.DestroyImmediate(root);
        return prefab.GetComponent<ScanSuccessUI>();
    }

    private static void CreateStartScene(GameObject managerPrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "StartScene";

        CreateMainCamera();
        CreateEventSystem();
        PrefabUtility.InstantiatePrefab(managerPrefab);

        GameObject canvas = CreateCanvas("Canvas_Start");
        GameObject startRoot = CreatePanel("StartUI", canvas.transform, new Color(0.1f, 0.16f, 0.22f, 1f));
        StartUI startUI = startRoot.AddComponent<StartUI>();

        GameObject title = CreateText("TitleText", startRoot.transform, "AR 扫描收集游戏", 38, TextAnchor.MiddleCenter);
        SetRect(title, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(520f, 80f), Vector2.zero);

        GameObject startButton = CreateButton("StartButton", startRoot.transform, "开始游戏");
        SetRect(startButton, new Vector2(0.5f, 0.48f), new Vector2(0.5f, 0.48f), new Vector2(260f, 70f), Vector2.zero);
        startUI.startButton = startButton.GetComponent<Button>();

        GameObject quitButton = CreateButton("QuitButton", startRoot.transform, "退出游戏");
        SetRect(quitButton, new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.34f), new Vector2(260f, 70f), Vector2.zero);
        startUI.quitButton = quitButton.GetComponent<Button>();

        PrefabUtility.SaveAsPrefabAsset(startRoot, UiPrefabPath + "/StartUI.prefab");
        EditorSceneManager.SaveScene(scene, ScenePath + "/StartScene.unity");
    }

    private static void CreateARScanScene(GameObject managerPrefab, TaskData task, ItemData item01, ItemData item02, ItemData item03, InventorySlotUI slotPrefab, ScanSuccessUI successPrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "ARScanScene";

        GameObject camera = CreateMainCamera();
        camera.name = "ARCamera";
        CreateEventSystem();
        PrefabUtility.InstantiatePrefab(managerPrefab);

        GameObject scanManagerObj = new GameObject("ScanManager");
        ScanManager scanManager = scanManagerObj.AddComponent<ScanManager>();

        GameObject canvas = CreateCanvas("Canvas_AR");
        GameObject scanUiObj = CreateScanUI(canvas.transform);
        GameObject taskUiObj = CreateTaskUI(canvas.transform);
        GameObject inventoryUiObj = CreateInventoryUI(canvas.transform, slotPrefab);

        ScanSuccessUI successUi = (ScanSuccessUI)PrefabUtility.InstantiatePrefab(successPrefab, canvas.transform);
        successUi.gameObject.name = "SuccessPanel";
        SetRect(successUi.gameObject, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), new Vector2(420f, 120f), Vector2.zero);
        scanManager.scanSuccessUI = successUi;

        PrefabUtility.SaveAsPrefabAsset(scanUiObj, UiPrefabPath + "/ScanUI.prefab");
        PrefabUtility.SaveAsPrefabAsset(taskUiObj, UiPrefabPath + "/TaskUI.prefab");
        PrefabUtility.SaveAsPrefabAsset(inventoryUiObj, UiPrefabPath + "/InventoryUI.prefab");

        GameObject imageTargets = new GameObject("ImageTargets");
        CreateImageTarget("ImageTarget_Task", imageTargets.transform, ScanTargetType.Task, "task_main_target", task, null, new Vector3(-2.4f, 0f, 0f));
        CreateImageTarget("ImageTarget_Item_01", imageTargets.transform, ScanTargetType.Item, item01.targetId, null, item01, new Vector3(-0.8f, 0f, 0f));
        CreateImageTarget("ImageTarget_Item_02", imageTargets.transform, ScanTargetType.Item, item02.targetId, null, item02, new Vector3(0.8f, 0f, 0f));
        CreateImageTarget("ImageTarget_Item_03", imageTargets.transform, ScanTargetType.Item, item03.targetId, null, item03, new Vector3(2.4f, 0f, 0f));

        EditorSceneManager.SaveScene(scene, ScenePath + "/ARScanScene.unity");
    }

    private static void CreateRewardScene(GameObject managerPrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "RewardScene";

        CreateMainCamera();
        CreateEventSystem();
        PrefabUtility.InstantiatePrefab(managerPrefab);

        GameObject canvas = CreateCanvas("Canvas_Reward");
        GameObject rewardRoot = CreatePanel("RewardUI", canvas.transform, new Color(0.12f, 0.13f, 0.18f, 1f));
        RewardUI rewardUI = rewardRoot.AddComponent<RewardUI>();

        GameObject title = CreateText("RewardTitleText", rewardRoot.transform, "比赛奖励", 36, TextAnchor.MiddleCenter);
        SetRect(title, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(520f, 70f), Vector2.zero);
        rewardUI.rewardTitleText = title.GetComponent<Text>();

        GameObject desc = CreateText("RewardDescText", rewardRoot.transform, "完成任务后可以兑换奖励", 22, TextAnchor.MiddleCenter);
        SetRect(desc, new Vector2(0.5f, 0.56f), new Vector2(0.5f, 0.56f), new Vector2(620f, 80f), Vector2.zero);
        rewardUI.rewardDescText = desc.GetComponent<Text>();

        GameObject result = CreateText("ResultText", rewardRoot.transform, "可以兑换奖励", 20, TextAnchor.MiddleCenter);
        SetRect(result, new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.42f), new Vector2(420f, 50f), Vector2.zero);
        rewardUI.resultText = result.GetComponent<Text>();

        GameObject exchangeButton = CreateButton("ExchangeButton", rewardRoot.transform, "兑换奖励");
        SetRect(exchangeButton, new Vector2(0.5f, 0.29f), new Vector2(0.5f, 0.29f), new Vector2(240f, 64f), Vector2.zero);
        rewardUI.exchangeButton = exchangeButton.GetComponent<Button>();

        GameObject backButton = CreateButton("BackHomeButton", rewardRoot.transform, "返回首页");
        SetRect(backButton, new Vector2(0.5f, 0.17f), new Vector2(0.5f, 0.17f), new Vector2(240f, 64f), Vector2.zero);
        rewardUI.backHomeButton = backButton.GetComponent<Button>();

        PrefabUtility.SaveAsPrefabAsset(rewardRoot, UiPrefabPath + "/RewardUI.prefab");
        EditorSceneManager.SaveScene(scene, ScenePath + "/RewardScene.unity");
    }

    private static void UpdateBuildSettings()
    {
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene(ScenePath + "/StartScene.unity", true),
            new EditorBuildSettingsScene(ScenePath + "/ARScanScene.unity", true),
            new EditorBuildSettingsScene(ScenePath + "/RewardScene.unity", true)
        };
    }

    private static GameObject CreateImageTarget(string name, Transform parent, ScanTargetType type, string targetId, TaskData task, ItemData item, Vector3 localPosition)
    {
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent);
        root.transform.localPosition = localPosition;

        ImageTargetScanHandler handler = root.AddComponent<ImageTargetScanHandler>();
        handler.targetType = type;
        handler.targetId = targetId;
        handler.taskData = task;
        handler.itemData = item;

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "VisualPlaceholder";
        visual.transform.SetParent(root.transform);
        visual.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        visual.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        handler.successVisual = visual;

        TextMesh textMesh = visual.AddComponent<TextMesh>();
        textMesh.text = type == ScanTargetType.Task ? "任务目标" : item.itemName;
        textMesh.characterSize = 0.12f;
        textMesh.anchor = TextAnchor.MiddleCenter;

        if (type == ScanTargetType.Task)
        {
            PrefabUtility.SaveAsPrefabAsset(root, ArPrefabPath + "/ImageTarget_Task.prefab");
        }
        else if (name == "ImageTarget_Item_01")
        {
            PrefabUtility.SaveAsPrefabAsset(root, ArPrefabPath + "/ImageTarget_Item.prefab");
        }

        return root;
    }

    private static GameObject CreateMainCamera()
    {
        GameObject camera = new GameObject("Main Camera");
        camera.tag = "MainCamera";
        Camera cam = camera.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.1f, 0.12f, 1f);
        camera.transform.position = new Vector3(0f, 0f, -10f);
        return camera;
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static GameObject CreateCanvas(string name)
    {
        GameObject canvas = new GameObject(name);
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        canvas.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panel = CreateUIObject(name);
        panel.transform.SetParent(parent, false);
        SetRectStretch(panel, Vector2.zero, Vector2.zero);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private static GameObject CreateScanUI(Transform parent)
    {
        GameObject root = CreateUIObject("ScanUI");
        root.transform.SetParent(parent, false);
        SetRectStretch(root, Vector2.zero, Vector2.zero);

        ScanUI scanUI = root.AddComponent<ScanUI>();
        GameObject hint = CreateText("ScanHintText", root.transform, "请对准任务图片开始扫描", 24, TextAnchor.MiddleCenter);
        SetRect(hint, new Vector2(0.5f, 0.92f), new Vector2(0.5f, 0.92f), new Vector2(780f, 60f), Vector2.zero);
        scanUI.scanHintText = hint.GetComponent<Text>();

        GameObject state = CreateText("ScanStateText", root.transform, "等待扫描", 22, TextAnchor.MiddleCenter);
        SetRect(state, new Vector2(0.5f, 0.86f), new Vector2(0.5f, 0.86f), new Vector2(520f, 50f), Vector2.zero);
        scanUI.scanStateText = state.GetComponent<Text>();

        return root;
    }

    private static GameObject CreateTaskUI(Transform parent)
    {
        GameObject root = CreatePanel("TaskUI", parent, new Color(0f, 0f, 0f, 0.45f));
        SetRect(root, new Vector2(0.5f, 0.76f), new Vector2(0.5f, 0.76f), new Vector2(760f, 170f), Vector2.zero);

        TaskUI taskUI = root.AddComponent<TaskUI>();
        GameObject name = CreateText("TaskNameText", root.transform, "尚未领取任务", 24, TextAnchor.MiddleCenter);
        SetRect(name, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(700f, 36f), Vector2.zero);
        taskUI.taskNameText = name.GetComponent<Text>();

        GameObject desc = CreateText("TaskDescText", root.transform, "请先扫描任务目标", 18, TextAnchor.MiddleCenter);
        SetRect(desc, new Vector2(0.5f, 0.46f), new Vector2(0.5f, 0.46f), new Vector2(700f, 48f), Vector2.zero);
        taskUI.taskDescText = desc.GetComponent<Text>();

        GameObject progress = CreateText("ProgressText", root.transform, "进度：0/0", 20, TextAnchor.MiddleCenter);
        SetRect(progress, new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), new Vector2(700f, 36f), Vector2.zero);
        taskUI.progressText = progress.GetComponent<Text>();

        return root;
    }

    private static GameObject CreateInventoryUI(Transform parent, InventorySlotUI slotPrefab)
    {
        GameObject root = CreatePanel("InventoryUI", parent, new Color(0f, 0f, 0f, 0.45f));
        SetRect(root, new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), new Vector2(820f, 210f), Vector2.zero);

        GameObject title = CreateText("InventoryTitleText", root.transform, "道具栏", 22, TextAnchor.MiddleCenter);
        SetRect(title, new Vector2(0.5f, 0.86f), new Vector2(0.5f, 0.86f), new Vector2(240f, 34f), Vector2.zero);

        GameObject slotRoot = CreateUIObject("SlotRoot");
        slotRoot.transform.SetParent(root.transform, false);
        SetRect(slotRoot, new Vector2(0.5f, 0.42f), new Vector2(0.5f, 0.42f), new Vector2(760f, 140f), Vector2.zero);
        HorizontalLayoutGroup layout = slotRoot.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        InventoryUI inventoryUI = root.AddComponent<InventoryUI>();
        inventoryUI.slotRoot = slotRoot.transform;
        inventoryUI.slotPrefab = slotPrefab;
        return root;
    }

    private static GameObject CreateButton(string name, Transform parent, string label)
    {
        GameObject buttonObj = CreateUIObject(name);
        buttonObj.transform.SetParent(parent, false);

        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.18f, 0.48f, 0.86f, 1f);

        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.24f, 0.58f, 0.95f, 1f);
        colors.pressedColor = new Color(0.12f, 0.34f, 0.62f, 1f);
        button.colors = colors;

        GameObject textObj = CreateText("Text", buttonObj.transform, label, 24, TextAnchor.MiddleCenter);
        SetRectStretch(textObj, Vector2.zero, Vector2.zero);

        return buttonObj;
    }

    private static GameObject CreateText(string name, Transform parent, string text, int fontSize, TextAnchor anchor)
    {
        GameObject textObj = CreateUIObject(name);
        textObj.transform.SetParent(parent, false);

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = fontSize;
        textComponent.alignment = anchor;
        textComponent.color = Color.white;
        textComponent.raycastTarget = false;
        return textObj;
    }

    private static GameObject CreateUIObject(string name)
    {
        GameObject obj = new GameObject(name);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private static void SetRect(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 size, Vector2 anchoredPosition)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;
    }

    private static void SetRectStretch(GameObject obj, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
