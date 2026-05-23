# Unity + Vuforia AR 扫描收集游戏实施说明

## 一键生成示例工程

1. 用 Unity 2022.3 LTS 或以上版本打开本目录。
2. 等待脚本编译完成。
3. 点击菜单 `AR Scan Game -> Generate Demo Project`。
4. 工具会生成：
   - `Assets/_Project/Data/Items/Item_01.asset` 到 `Item_03.asset`
   - `Assets/_Project/Data/Tasks/Task_Main.asset`
   - `Assets/_Project/Prefabs/Managers/GameManagers.prefab`
   - UI Prefab 和 AR 占位 Prefab
   - `StartScene`、`ARScanScene`、`RewardScene`
5. 工具会自动把三个场景加入 `File -> Build Settings`：
   - `StartScene`
   - `ARScanScene`
   - `RewardScene`

## Vuforia 配置步骤

1. 到 Vuforia Developer Portal 下载 Vuforia Engine Unity Extension。
2. 在 Unity 中选择 `Assets -> Import Package -> Custom Package...` 导入 Vuforia 包。
3. 打开 `ARScanScene`。
4. 删除示例里的普通 `ARCamera`，选择 `GameObject -> Vuforia Engine -> AR Camera` 创建真实 ARCamera。
5. 选中 ARCamera，在 Inspector 中打开 Vuforia Configuration，填写 License Key。
6. 到 Vuforia Target Manager 上传 `Assets/_Project/Art/images/` 中的 JPG/PNG 图片。
7. 下载 Device Database 并导入 Unity。
8. 为每张图片创建 `GameObject -> Vuforia Engine -> Image Target`。
9. 在 ImageTarget 上选择对应 Database 和 Target。
10. 给每个 ImageTarget 挂载 `ImageTargetScanHandler`：
    - 任务目标：`Target Type = Task`，绑定 `Task_Main.asset`
    - 道具目标：`Target Type = Item`，绑定对应 `Item_XX.asset`
11. 打开 `Project Settings -> Player -> Other Settings -> Scripting Define Symbols`，添加：

```text
VUFORIA_PRESENT
```

添加后，`ImageTargetScanHandler` 会启用真实 Vuforia 识别回调。

## ImageTarget 预制体结构

```text
ImageTarget_Item_01
├─ ImageTargetBehaviour
├─ ImageTargetScanHandler
│  ├─ targetType = Item
│  ├─ targetId = item_01_target
│  └─ itemData = Item_01
└─ VisualPlaceholder
   └─ 用于识别成功后显示的 Cube / 模型 / 文字
```

```text
ImageTarget_Task
├─ ImageTargetBehaviour
├─ ImageTargetScanHandler
│  ├─ targetType = Task
│  ├─ targetId = task_main_target
│  └─ taskData = Task_Main
└─ VisualPlaceholder
   └─ 用于识别成功后显示任务领取提示
```

## 扫描成功 UI

`ScanManager` 引用 `ScanSuccessUI`。

扫描成功时会显示：

```text
任务已领取：收集兑换道具
获得道具：钥匙
这个目标已经扫描过了
请先扫描任务目标领取任务
```

UI 动画使用 `CanvasGroup` 淡入淡出和 `RectTransform` 缩放，代码在 `ScanSuccessUI.cs`。

## 示例识别逻辑

真实 Vuforia 扫描流程：

```text
Vuforia ImageTarget 被识别
-> ImageTargetScanHandler.OnTargetStatusChanged
-> ScanManager.HandleTargetScanned
-> GameEvents.RaiseTaskScanned 或 GameEvents.RaiseItemScanned
-> TaskManager / InventoryManager 更新数据
-> TaskUI / InventoryUI / ScanUI 刷新显示
```

未接 Vuforia 时测试：

1. 打开 `ARScanScene`。
2. 选中某个 `ImageTarget_XXX`。
3. 在 `ImageTargetScanHandler` 组件右键选择 `模拟扫描成功`。

## 脚本挂载位置

```text
GameManagers.prefab
├─ GameManager
├─ TaskManager
├─ InventoryManager
└─ RewardManager
```

```text
ARScanScene
├─ ScanManager
├─ Canvas_AR/ScanUI
├─ Canvas_AR/TaskUI
├─ Canvas_AR/InventoryUI
└─ ImageTargets/*/ImageTargetScanHandler
```

```text
StartScene
└─ Canvas_Start/StartUI
```

```text
RewardScene
└─ Canvas_Reward/RewardUI
```

## 开发顺序

1. 运行一键生成示例工程。
2. 测试不接 Vuforia 的模拟扫描流程。
3. 导入 Vuforia 并配置 ARCamera。
4. 创建 Vuforia Target Database。
5. 把示例 ImageTarget 替换成真实 Vuforia ImageTarget。
6. 安卓真机测试摄像头权限和识别效果。
7. 替换 `dg` 里的道具图标或模型。
8. 调整 UI 样式和比赛展示文案。
