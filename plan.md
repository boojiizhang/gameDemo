# Unity + Vuforia 手机 AR 扫描收集游戏计划

## Summary

**本项目按“学生比赛快速开发”风格搭建：Unity 2022.3 LTS+、Android、URP、Canvas UI、Vuforia ImageTarget。核心流程是首页开始、进入 AR 扫描、先扫描任务目标领取任务、继续扫描多个道具目标、道具进入背包、进度更新、全部收集后进入奖励兑换场景。**

**当前仓库还没有 Unity 工程结构，因此计划会从标准 Unity 项目目录开始规划。Vuforia 接入采用官方推荐的 ARCamera + Image Target + Target Database 方式；依据参考：**[Vuforia Unity Getting Started](https://developer.vuforia.com/library/vuforia-engine/getting-started/development-environments/getting-started-vuforia-engine-unity/?partnerID=Unity)、[Vuforia Image Targets](https://developer.vuforia.com/library/vuforia-engine/images-and-objects/image-targets/image-targets/)、[Vuforia Unity Lifecycle](https://developer.vuforia.com/library/vuforia-engine/getting-started/vuforia-engine-api/vuforia-engine-api-unity/)。

## 完整项目目录树

```
gameDemo/
├─ Assets/
│  ├─ _Project/
│  │  ├─ Scenes/
│  │  │  ├─ StartScene.unity
│  │  │  ├─ ARScanScene.unity
│  │  │  └─ RewardScene.unity
│  │  ├─ Scripts/
│  │  │  ├─ Core/
│  │  │  │  ├─ GameManager.cs
│  │  │  │  └─ GameEvents.cs
│  │  │  ├─ Scan/
│  │  │  │  ├─ ScanManager.cs
│  │  │  │  └─ ImageTargetScanHandler.cs
│  │  │  ├─ Task/
│  │  │  │  ├─ TaskManager.cs
│  │  │  │  └─ TaskUI.cs
│  │  │  ├─ Inventory/
│  │  │  │  ├─ InventoryManager.cs
│  │  │  │  ├─ InventoryUI.cs
│  │  │  │  └─ InventorySlotUI.cs
│  │  │  ├─ Reward/
│  │  │  │  ├─ RewardManager.cs
│  │  │  │  └─ RewardUI.cs
│  │  │  └─ UI/
│  │  │     ├─ UIManager.cs
│  │  │     ├─ StartUI.cs
│  │  │     └─ ScanSuccessUI.cs
│  │  ├─ Data/
│  │  │  ├─ ScriptableObjects/
│  │  │  │  ├─ ItemData.cs
│  │  │  │  └─ TaskData.cs
│  │  │  ├─ Items/
│  │  │  │  ├─ Item_01.asset
│  │  │  │  ├─ Item_02.asset
│  │  │  │  └─ Item_03.asset
│  │  │  └─ Tasks/
│  │  │     └─ Task_Main.asset
│  │  ├─ Prefabs/
│  │  │  ├─ Managers/
│  │  │  │  └─ GameManagers.prefab
│  │  │  ├─ AR/
│  │  │  │  ├─ ImageTarget_Task.prefab
│  │  │  │  └─ ImageTarget_Item.prefab
│  │  │  └─ UI/
│  │  │     ├─ StartUI.prefab
│  │  │     ├─ TaskUI.prefab
│  │  │     ├─ InventoryUI.prefab
│  │  │     ├─ InventorySlotUI.prefab
│  │  │     ├─ ScanUI.prefab
│  │  │     ├─ ScanSuccessUI.prefab
│  │  │     └─ RewardUI.prefab
│  │  ├─ Art/
│  │  │  ├─ images/
│  │  │  │  └─ 扫描测试图片放这里
│  │  │  ├─ dg/
│  │  │  │  └─ 道具图片或模型放这里
│  │  │  └─ Placeholder/
│  │  │     └─ TextIconPlaceholder.png
│  │  └─ Settings/
│  │     └─ URP_Renderer.asset
│  ├─ Vuforia/
│  │  └─ Vuforia 导入后自动生成
│  └─ Resources/
│     └─ 可选：比赛演示用临时资源
├─ Packages/
├─ ProjectSettings/
└─ README.md
```

## 场景结构

`StartScene`

```
StartScene
├─ EventSystem
├─ Main Camera
├─ GameManagers
│  ├─ GameManager
│  ├─ TaskManager
│  ├─ InventoryManager
│  └─ RewardManager
└─ Canvas_Start
   └─ StartUI
      ├─ TitleText
      ├─ StartButton
      └─ QuitButton
```

`ARScanScene`

```
ARScanScene
├─ EventSystem
├─ ARCamera
│  └─ VuforiaBehaviour
├─ ScanManager
├─ Canvas_AR
│  ├─ ScanUI
│  │  ├─ ScanHintText
│  │  ├─ ScanStateText
│  │  └─ SuccessPanel
│  ├─ TaskUI
│  │  ├─ TaskNameText
│  │  ├─ TaskDescText
│  │  └─ ProgressText
│  └─ InventoryUI
│     ├─ SlotRoot
│     └─ InventorySlotUI x N
└─ ImageTargets
   ├─ ImageTarget_Task
   │  ├─ ImageTargetBehaviour
   │  ├─ DefaultObserverEventHandler 或 ImageTargetScanHandler
   │  └─ TaskVisualPlaceholder
   ├─ ImageTarget_Item_01
   ├─ ImageTarget_Item_02
   └─ ImageTarget_Item_03
```

`RewardScene`

```
RewardScene
├─ EventSystem
├─ Main Camera
├─ Canvas_Reward
│  └─ RewardUI
│     ├─ RewardTitleText
│     ├─ RewardDescText
│     ├─ ExchangeButton
│     └─ BackHomeButton
└─ RewardManagerProxy 可选
```

**场景连接方式：**

* `StartUI.StartButton` 调用 `GameManager.LoadARScanScene()`。
* `ARScanScene` 中先扫描任务 ImageTarget，`ScanManager` 触发任务领取事件。
* **扫描道具 ImageTarget 后触发道具获得事件，**`InventoryManager` 增加道具，`TaskManager` 更新进度。
* `TaskManager` 判断任务完成后触发任务完成事件。
* `GameManager` 收到任务完成事件后加载 `RewardScene`。
* `RewardUI.ExchangeButton` 调用 `RewardManager.ExchangeReward()`，完成游戏流程。

## 核心系统与 Manager 架构

**采用简单 Manager + 静态事件类通信，不使用 ECS、DI、MVVM。**

```
GameManager
负责跨场景流程、DontDestroyOnLoad、加载 StartScene / ARScanScene / RewardScene。

ScanManager
只存在于 ARScanScene，负责监听 ImageTarget 扫描成功、去重、播放成功 UI、派发扫描事件。

TaskManager
跨场景保留，负责当前任务、领取任务、任务进度、任务完成判断。

InventoryManager
跨场景保留，负责已获得道具列表、查询道具是否已获得、防止重复添加。

UIManager
每个场景可独立存在，负责显示/隐藏当前场景 UI 面板，不存游戏数据。

RewardManager
跨场景或 RewardScene 内存在，负责奖励是否可兑换、兑换完成状态。
```

**需要 **`DontDestroyOnLoad` 的对象：

```
GameManagers.prefab
├─ GameManager
├─ TaskManager
├─ InventoryManager
└─ RewardManager
```

**不需要 **`DontDestroyOnLoad` 的对象：

```
ScanManager
UIManager
StartUI
ScanUI
TaskUI
InventoryUI
RewardUI
ImageTargetScanHandler
```

## Script 命名与挂载位置

```
GameManager.cs
挂载：GameManagers.prefab / GameManager
职责：单例、场景加载、游戏开始、游戏结束。

GameEvents.cs
挂载：不挂载
职责：静态事件中心，例如 OnTaskAccepted、OnItemScanned、OnItemAdded、OnTaskCompleted。

ScanManager.cs
挂载：ARScanScene / ScanManager
职责：接收 ImageTargetScanHandler 的扫描成功回调，判断是否重复扫描，派发道具或任务事件。

ImageTargetScanHandler.cs
挂载：每个 ImageTarget prefab
职责：监听 Vuforia 识别状态，把 targetId、ItemData 或 TaskData 交给 ScanManager。

TaskManager.cs
挂载：GameManagers.prefab / TaskManager
职责：保存当前 TaskData、领取任务、统计进度、判断完成。

TaskData.cs
挂载：不挂载，ScriptableObject
职责：任务名称、任务说明、需要收集的 ItemData 列表、奖励描述。

TaskUI.cs
挂载：ARScanScene / Canvas_AR / TaskUI
职责：监听任务事件并刷新任务标题、描述、进度文字。

InventoryManager.cs
挂载：GameManagers.prefab / InventoryManager
职责：保存已获得 ItemData 列表，提供 AddItem、HasItem、GetItems。

ItemData.cs
挂载：不挂载，ScriptableObject
职责：道具 ID、道具名称、图标、扫描目标 ID、说明。

InventoryUI.cs
挂载：ARScanScene / Canvas_AR / InventoryUI
职责：根据 TaskData 生成背包格子，监听道具获得事件刷新状态。

InventorySlotUI.cs
挂载：InventorySlotUI.prefab
职责：显示单个道具格子，已获得高亮，未获得灰色。

UIManager.cs
挂载：每个场景的 Canvas 或 UIManager
职责：统一开关面板，例如扫描成功弹窗、奖励弹窗。

StartUI.cs
挂载：StartUI.prefab
职责：首页按钮绑定。

ScanSuccessUI.cs
挂载：ScanSuccessUI.prefab
职责：显示“获得 xxx”文字，播放简单 Canvas 缩放或淡入动画。

RewardManager.cs
挂载：GameManagers.prefab / RewardManager
职责：检查是否可兑换、记录奖励兑换状态。

RewardUI.cs
挂载：RewardScene / Canvas_Reward / RewardUI
职责：显示奖励信息和兑换按钮。
```

## Prefab 结构

```
GameManagers.prefab
├─ GameManager
├─ TaskManager
├─ InventoryManager
└─ RewardManager
```

```
ImageTarget_Task.prefab
├─ ImageTargetBehaviour
├─ DefaultObserverEventHandler 或自定义识别监听
├─ ImageTargetScanHandler
│  ├─ TargetType = Task
│  ├─ TargetId = "task_main"
│  └─ TaskData = Task_Main
└─ VisualPlaceholder
   └─ TextMesh 或简单 Cube
```

```
ImageTarget_Item.prefab
├─ ImageTargetBehaviour
├─ DefaultObserverEventHandler 或自定义识别监听
├─ ImageTargetScanHandler
│  ├─ TargetType = Item
│  ├─ TargetId = "item_01"
│  └─ ItemData = Item_01
└─ VisualPlaceholder
   └─ TextMesh 或简单 Cube
```

```
InventorySlotUI.prefab
├─ BackgroundImage
├─ IconImage
├─ NameText
└─ StateText
```

```
ScanSuccessUI.prefab
├─ Panel
│  ├─ TitleText
│  ├─ ItemNameText
│  └─ CloseAfterDelayAnimation
```

## UI 层级

```
Canvas_Start
└─ StartUI
   ├─ TitleText
   ├─ StartButton
   └─ QuitButton
```

```
Canvas_AR
├─ ScanUI
│  ├─ TopHintText
│  ├─ CenterScanFrame
│  └─ SuccessPanel
├─ TaskUI
│  ├─ TaskNameText
│  ├─ TaskDescText
│  └─ ProgressText
└─ InventoryUI
   ├─ InventoryTitleText
   └─ SlotRoot
      ├─ InventorySlotUI
      ├─ InventorySlotUI
      └─ InventorySlotUI
```

```
Canvas_Reward
└─ RewardUI
   ├─ RewardTitleText
   ├─ RewardDescText
   ├─ ExchangeButton
   ├─ ResultText
   └─ BackHomeButton
```

**没有图片时使用文字占位：**

* **道具图标为空时显示 **`IconText = 道具名首字`。
* **扫描目标展示用 **`TextMesh` 或 UI 文本写“任务目标 / 道具 01”。
* **扫描成功动画使用 CanvasGroup 淡入淡出 + RectTransform 轻微缩放。**

## ScriptableObject 数据

`ItemData` 字段：

```
itemId：唯一道具 ID，例如 item_01
itemName：道具名称
description：道具说明
icon：道具图标，可为空
targetId：对应 Vuforia ImageTarget ID
```

`TaskData` 字段：

```
taskId：任务 ID，例如 task_main
taskName：任务名称
description：任务说明
requiredItems：需要收集的 ItemData 列表
rewardName：奖励名称
rewardDescription：奖励说明
```

**示例数据：**

```
Task_Main.asset
├─ taskId = task_main
├─ taskName = 收集兑换道具
├─ requiredItems = [Item_01, Item_02, Item_03]
└─ rewardName = 比赛奖励

Item_01.asset
├─ itemId = item_01
├─ itemName = 道具一
└─ targetId = item_01_target
```

## 数据流结构

<pre class="md-fences md-end-block md-diagram md-fences-advanced ty-contain-cm modeLoaded" spellcheck="false" lang="mermaid" cid="n63" mdtype="fences" mermaid-type="flowchart"><div class="md-diagram-panel md-fences-adv-panel"><div class="md-diagram-panel-header md-fences-adv-panel-header"></div><div class="md-diagram-panel-preview md-fences-adv-panel-preview"><svg id="mermaidChart0" width="100%" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="-8 -8 584.31640625 1790.875" role="graphics-document document" aria-roledescription="flowchart-v2" class="in-text-selection"><g><marker id="mermaidChart0_flowchart-pointEnd" class="marker flowchart" viewBox="0 0 10 10" refX="6" refY="5" markerUnits="userSpaceOnUse" markerWidth="12" markerHeight="12" orient="auto"><path d="M 0 0 L 10 5 L 0 10 z" class="arrowMarkerPath"></path></marker><marker id="mermaidChart0_flowchart-pointStart" class="marker flowchart" viewBox="0 0 10 10" refX="4.5" refY="5" markerUnits="userSpaceOnUse" markerWidth="12" markerHeight="12" orient="auto"><path d="M 0 5 L 10 10 L 10 0 z" class="arrowMarkerPath"></path></marker><marker id="mermaidChart0_flowchart-circleEnd" class="marker flowchart" viewBox="0 0 10 10" refX="11" refY="5" markerUnits="userSpaceOnUse" markerWidth="11" markerHeight="11" orient="auto"><circle cx="5" cy="5" r="5" class="arrowMarkerPath"></circle></marker><marker id="mermaidChart0_flowchart-circleStart" class="marker flowchart" viewBox="0 0 10 10" refX="-1" refY="5" markerUnits="userSpaceOnUse" markerWidth="11" markerHeight="11" orient="auto"><circle cx="5" cy="5" r="5" class="arrowMarkerPath"></circle></marker><marker id="mermaidChart0_flowchart-crossEnd" class="marker cross flowchart" viewBox="0 0 11 11" refX="12" refY="5.2" markerUnits="userSpaceOnUse" markerWidth="11" markerHeight="11" orient="auto"><path d="M 1,1 l 9,9 M 10,1 l -9,9" class="arrowMarkerPath"></path></marker><marker id="mermaidChart0_flowchart-crossStart" class="marker cross flowchart" viewBox="0 0 11 11" refX="-1" refY="5.2" markerUnits="userSpaceOnUse" markerWidth="11" markerHeight="11" orient="auto"><path d="M 1,1 l 9,9 M 10,1 l -9,9" class="arrowMarkerPath"></path></marker><g class="root"><g class="clusters"></g><g class="edgePaths"><path d="M305.895,40.594L305.895,65.594L305.895,85.294" id="L-A-B-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-A LE-B" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,131.188L305.895,156.188L305.895,175.888" id="L-B-C-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-B LE-C" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,221.781L305.895,246.781L305.895,266.481" id="L-C-D-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-C LE-D" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,312.375L305.895,337.375L305.895,357.075" id="L-D-E-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-D LE-E" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,402.969L305.895,427.969L305.895,447.669" id="L-E-F-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-E LE-F" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,493.563L305.895,518.563L305.895,538.263" id="L-F-G-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-F LE-G" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,584.156L305.895,609.156L305.895,628.856" id="L-G-H-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-G LE-H" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M305.895,674.75L305.895,699.75L305.895,719.45" id="L-H-I-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-H LE-I" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M267.967,765.344L221.25,790.344L221.25,810.044" id="L-I-J-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-I LE-J" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M221.25,855.938L221.25,880.938L221.25,900.638" id="L-J-K-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-J LE-K" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M221.25,946.531L221.25,984.328L221.25,1016.825" id="L-K-L-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-K LE-L" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M164.557,1062.719L94.727,1087.719L94.727,1107.419" id="L-L-M-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-L LE-M" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M277.943,1062.719L347.773,1087.719L347.773,1107.419" id="L-L-N-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-L LE-N" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M347.773,1153.313L347.773,1178.313L391.229,1238.098" id="L-N-O-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-N LE-O" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M467.823,1238.718L504.078,1178.313L504.078,1133.016L504.078,1087.719L504.078,1042.422L504.078,984.328L504.078,926.234L504.078,880.938L504.078,835.641L504.078,790.344L399.864,766.525" id="L-O-I-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-O LE-I" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M432.918,1387.406L432.418,1424.703L432.418,1457.2" id="L-O-P-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-O LE-P" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M432.418,1503.094L432.418,1528.094L432.418,1547.794" id="L-P-Q-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-P LE-Q" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M432.418,1593.688L432.418,1618.688L432.418,1638.388" id="L-Q-R-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-Q LE-R" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path><path d="M432.418,1684.281L432.418,1709.281L432.418,1728.981" id="L-R-S-0" class=" edge-thickness-normal edge-pattern-solid flowchart-link LS-R LE-S" marker-end="url(#mermaidChart0_flowchart-pointEnd)"></path></g><g class="edgeLabels"><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel" transform="translate(504.078125, 984.328125)"><g class="label" transform="translate(-8, -12.796875)"><foreignObject width="16" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel">否</span></div></foreignObject></g></g><g class="edgeLabel" transform="translate(432.41796875, 1424.703125)"><g class="label" transform="translate(-8, -12.796875)"><foreignObject width="16" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel">是</span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g><g class="edgeLabel"><g class="label" transform="translate(0, 0)"><foreignObject width="0" height="0"><div xmlns="http://www.w3.org/1999/xhtml"><span class="edgeLabel"></span></div></foreignObject></g></g></g><g class="nodes"><g class="node default default flowchart-label" id="flowchart-A-0" data-node="true" data-id="A" transform="translate(305.89453125, 20.296875)"><rect class="basic label-container" rx="0" ry="0" x="-116.4609375" y="-20.296875" width="232.921875" height="40.59375"></rect><g class="label" transform="translate(-108.9609375, -12.796875)"><rect></rect><foreignObject width="217.921875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">StartScene: 玩家点击开始游戏</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-B-1" data-node="true" data-id="B" transform="translate(305.89453125, 110.890625)"><rect class="basic label-container" rx="0" ry="0" x="-136.4140625" y="-20.296875" width="272.828125" height="40.59375"></rect><g class="label" transform="translate(-128.9140625, -12.796875)"><rect></rect><foreignObject width="257.828125" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">GameManager 加载 ARScanScene</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-C-3" data-node="true" data-id="C" transform="translate(305.89453125, 201.484375)"><rect class="basic label-container" rx="0" ry="0" x="-119.734375" y="-20.296875" width="239.46875" height="40.59375"></rect><g class="label" transform="translate(-112.234375, -12.796875)"><rect></rect><foreignObject width="224.46875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">Vuforia ARCamera 打开摄像头</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-D-5" data-node="true" data-id="D" transform="translate(305.89453125, 292.078125)"><rect class="basic label-container" rx="0" ry="0" x="-89.46875" y="-20.296875" width="178.9375" height="40.59375"></rect><g class="label" transform="translate(-81.96875, -12.796875)"><rect></rect><foreignObject width="163.9375" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">扫描任务 ImageTarget</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-E-7" data-node="true" data-id="E" transform="translate(305.89453125, 382.671875)"><rect class="basic label-container" rx="0" ry="0" x="-176.4765625" y="-20.296875" width="352.953125" height="40.59375"></rect><g class="label" transform="translate(-168.9765625, -12.796875)"><rect></rect><foreignObject width="337.953125" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">ImageTargetScanHandler 通知 ScanManager</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-F-9" data-node="true" data-id="F" transform="translate(305.89453125, 473.265625)"><rect class="basic label-container" rx="0" ry="0" x="-140.6875" y="-20.296875" width="281.375" height="40.59375"></rect><g class="label" transform="translate(-133.1875, -12.796875)"><rect></rect><foreignObject width="266.375" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">ScanManager 触发 OnTaskScanned</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-G-11" data-node="true" data-id="G" transform="translate(305.89453125, 563.859375)"><rect class="basic label-container" rx="0" ry="0" x="-112.7578125" y="-20.296875" width="225.515625" height="40.59375"></rect><g class="label" transform="translate(-105.2578125, -12.796875)"><rect></rect><foreignObject width="210.515625" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">TaskManager 领取 TaskData</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-H-13" data-node="true" data-id="H" transform="translate(305.89453125, 654.453125)"><rect class="basic label-container" rx="0" ry="0" x="-82.34375" y="-20.296875" width="164.6875" height="40.59375"></rect><g class="label" transform="translate(-74.84375, -12.796875)"><rect></rect><foreignObject width="149.6875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">TaskUI 显示任务信息</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-I-15" data-node="true" data-id="I" transform="translate(305.89453125, 745.046875)"><rect class="basic label-container" rx="0" ry="0" x="-105.46875" y="-20.296875" width="210.9375" height="40.59375"></rect><g class="label" transform="translate(-97.96875, -12.796875)"><rect></rect><foreignObject width="195.9375" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">继续扫描道具 ImageTarget</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-J-17" data-node="true" data-id="J" transform="translate(221.25, 835.640625)"><rect class="basic label-container" rx="0" ry="0" x="-126.1484375" y="-20.296875" width="252.296875" height="40.59375"></rect><g class="label" transform="translate(-118.6484375, -12.796875)"><rect></rect><foreignObject width="237.296875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">ScanManager 检查是否重复扫描</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-K-19" data-node="true" data-id="K" transform="translate(221.25, 926.234375)"><rect class="basic label-container" rx="0" ry="0" x="-87.1875" y="-20.296875" width="174.375" height="40.59375"></rect><g class="label" transform="translate(-79.6875, -12.796875)"><rect></rect><foreignObject width="159.375" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">触发 OnItemScanned</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-L-21" data-node="true" data-id="L" transform="translate(221.25, 1042.421875)"><rect class="basic label-container" rx="0" ry="0" x="-134.2890625" y="-20.296875" width="268.578125" height="40.59375"></rect><g class="label" transform="translate(-126.7890625, -12.796875)"><rect></rect><foreignObject width="253.578125" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">InventoryManager 添加 ItemData</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-M-23" data-node="true" data-id="M" transform="translate(94.7265625, 1133.015625)"><rect class="basic label-container" rx="0" ry="0" x="-94.7265625" y="-20.296875" width="189.453125" height="40.59375"></rect><g class="label" transform="translate(-87.2265625, -12.796875)"><rect></rect><foreignObject width="174.453125" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">InventoryUI 刷新道具栏</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-N-25" data-node="true" data-id="N" transform="translate(347.7734375, 1133.015625)"><rect class="basic label-container" rx="0" ry="0" x="-108.3203125" y="-20.296875" width="216.640625" height="40.59375"></rect><g class="label" transform="translate(-100.8203125, -12.796875)"><rect></rect><foreignObject width="201.640625" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">TaskManager 更新任务进度</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-O-27" data-node="true" data-id="O" transform="translate(432.41796875, 1295.109375)"><polygon points="91.796875,0 183.59375,-91.796875 91.796875,-183.59375 0,-91.796875" class="label-container" transform="translate(-91.796875,91.796875)"></polygon><g class="label" transform="translate(-64, -12.796875)"><rect></rect><foreignObject width="128" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">是否收集全部道具</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-P-31" data-node="true" data-id="P" transform="translate(432.41796875, 1482.796875)"><rect class="basic label-container" rx="0" ry="0" x="-95.75" y="-20.296875" width="191.5" height="40.59375"></rect><g class="label" transform="translate(-88.25, -12.796875)"><rect></rect><foreignObject width="176.5" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">触发 OnTaskCompleted</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-Q-33" data-node="true" data-id="Q" transform="translate(432.41796875, 1573.390625)"><rect class="basic label-container" rx="0" ry="0" x="-135.8984375" y="-20.296875" width="271.796875" height="40.59375"></rect><g class="label" transform="translate(-128.3984375, -12.796875)"><rect></rect><foreignObject width="256.796875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">GameManager 加载 RewardScene</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-R-35" data-node="true" data-id="R" transform="translate(432.41796875, 1663.984375)"><rect class="basic label-container" rx="0" ry="0" x="-94.5078125" y="-20.296875" width="189.015625" height="40.59375"></rect><g class="label" transform="translate(-87.0078125, -12.796875)"><rect></rect><foreignObject width="174.015625" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">RewardUI 显示兑换奖励</span></div></foreignObject></g></g><g class="node default default flowchart-label" id="flowchart-S-37" data-node="true" data-id="S" transform="translate(432.41796875, 1754.578125)"><rect class="basic label-container" rx="0" ry="0" x="-104.484375" y="-20.296875" width="208.96875" height="40.59375"></rect><g class="label" transform="translate(-96.984375, -12.796875)"><rect></rect><foreignObject width="193.96875" height="25.59375"><div xmlns="http://www.w3.org/1999/xhtml"><span class="nodeLabel">RewardManager 完成兑换</span></div></foreignObject></g></g></g></g></g></svg></div><div class="md-diagram-panel-error md-fences-adv-panel-error"></div></div></pre>

**事件系统：**

```
GameEvents.OnTaskScanned(TaskData taskData)
GameEvents.OnTaskAccepted(TaskData taskData)
GameEvents.OnItemScanned(ItemData itemData)
GameEvents.OnItemAdded(ItemData itemData)
GameEvents.OnTaskProgressChanged(int current, int total)
GameEvents.OnTaskCompleted(TaskData taskData)
GameEvents.OnRewardExchanged()
```

## Vuforia 接入计划

* **通过 Vuforia Developer Portal 下载 Unity Extension，使用 **`Assets -> Import Package -> Custom Package...` 导入。
* **在 **`ARScanScene` 中添加 `GameObject -> Vuforia Engine -> AR Camera`，删除普通 Main Camera。
* **在 ARCamera 的 Vuforia Configuration 中填写 License Key。**
* **使用 Vuforia Target Manager 上传 **`Assets/_Project/Art/images/` 中的 JPG/PNG，生成 Device Database 并导入 Unity。
* **每张扫描图创建一个 ImageTarget，选择对应 Database 和 Target。**
* **每个 ImageTarget 挂载 **`ImageTargetScanHandler`，配置 `TargetType`、`TargetId`、`ItemData` 或 `TaskData`。
* **AR 内容作为 ImageTarget 子物体；比赛第一版只放文字或简单 Cube 占位。**
* **Android Build Settings 中切换 Android 平台，启用 Camera 权限，确认包名、最低 API、横竖屏方向。**
* **URP 可用，但只使用 Vuforia Engine Extension；不依赖 Vuforia Core Samples 的复杂资源。**

## 分阶段开发顺序

1. **创建 Unity 2022.3 LTS URP Android 项目，整理目录，建立三个 Scene。**
2. **搭建 **`GameManagers.prefab`，实现 `GameManager`、`GameEvents`、跨场景保留。
3. **创建 **`ItemData`、`TaskData` ScriptableObject，录入示例任务和道具。
4. **完成 **`StartScene`、`StartUI`，实现开始游戏进入 `ARScanScene`。
5. **接入 Vuforia，配置 ARCamera、License、ImageTarget、Android 摄像头权限。**
6. **实现 **`ScanManager.cs` 与 `ImageTargetScanHandler.cs`：扫描、去重、事件派发、扫描成功 UI。
7. **实现 **`TaskManager.cs` 与 `TaskUI.cs`：领取任务、显示进度、完成判断。
8. **实现 **`InventoryManager.cs`、`InventoryUI.cs`、`InventorySlotUI.cs`：获得道具、灰色/高亮状态、图标或文字占位。
9. **实现 **`RewardManager.cs` 与 `RewardUI.cs`：任务完成后进入奖励页并兑换。
10. **安卓真机测试：摄像头打开、每张图片识别、重复扫描不重复发道具、全部收集后跳转奖励页。**

## Test Plan

* **首页点击“开始游戏”后进入 **`ARScanScene`。
* **Android 真机启动 **`ARScanScene` 后摄像头正常打开。
* **扫描任务 ImageTarget 后只领取一次任务。**
* **扫描每个道具 ImageTarget 后只获得一次对应道具。**
* **重复扫描同一道具不会重复加入背包，也不会重复增加任务进度。**
* `InventoryUI` 中未获得道具为灰色，已获得道具高亮。
* `TaskUI` 进度从 `0/N` 正确更新到 `N/N`。
* **全部收集后自动进入 **`RewardScene`。
* **奖励页点击兑换后显示兑换完成状态。**
* **没有图标资源时 UI 使用文字占位，不报空引用错误。**

## Assumptions

* **第一版只做 ImageTarget 扫描，不做 3D Model Target；你文案里的“任务模型”按“任务扫描图/任务目标”处理。**
* `images` 文件夹用于 Vuforia 扫描图，`dg` 文件夹用于道具图标或模型资源；当前仓库未发现这两个文件夹，执行阶段会在 Unity `Assets/_Project/Art/` 下创建。
* **任务数量第一版为 1 个主任务，道具数量由 **`TaskData.requiredItems` 决定。
* **事件系统使用简单静态 C# event，便于新手理解和复制，不引入第三方事件框架。**
* **所有代码执行阶段会中文注释、每个系统独立文件、避免高级语法和伪代码。**
