# Eldrenn — Unity 2D RTS

## 项目信息
- **引擎**: Unity 2022 LTS (2D 模板)
- **仓库**: https://github.com/sail0kevin/Warriors
- **脚本路径**: Assets/Scripts/

## 主要系统
| 系统 | 关键文件 |
|------|---------|
| 通用面板管理 | PanelManager.cs |
| 面板切换按钮 | BuildingPanelToggle.cs |
| 单位控制/选择 | UnitController.cs, SelectionManager.cs |
| 战斗 | UnitCombat.cs, HealthSystem.cs |
| 治疗 | UnitHealer.cs |
| 采集 | UnitGatherer.cs, ResourceNode.cs |
| 资源管理 | ResourceManager.cs |
| 建造 | BuildingPlacementManager.cs, BuildingButton.cs |
| 招募 | RecruitSystem.cs |
| 玩家操作 | WarriorAct.cs, PlayerMovement.cs |
| 羊AI | SheepAI.cs |
| UI | UIManager.cs |
| 人口系统 | ResourceManager.cs, BuildingPopulation.cs, UnitPopulation.cs |
| 地图边界 | TilemapCollider2D + EdgeCollider2D（水上碰撞体） |

## 面板规则
- PanelManager 保证同时只有一个面板打开（支持 owner 追踪：同面板不同 owner 不会误关）
- 每个实体持有自己的面板引用，通过 `PanelManager.Instance.TogglePanel(panel, owner)` 控制
- BuildingPanelToggle 是通用 UI 按钮组件，挂任意按钮上，拖入 targetPanel 即可
- 招募面板初始必须取消勾选（游戏 Start 时自动关闭），按钮按子物体顺序自动绑定

## 人口规则
- 城堡 +50 人口上限，其他建筑各 +20
- 全局上限 200（POPULATION_CAP）
- 单位招募消耗人口，死亡释放人口
- 场景预置单位通过 UnitPopulation.Start() 自动扣人口
- 招募生成单位通过 RecruitSystem 标记 `recruited=true` 跳过重复扣除

## 采集规则
- 农民自动搜索最近资源 → 走过去采集 → 动画 + 资源入账
- 采集羊时：农民到采集距离 → 调 sheep.Freeze() 冻住羊 → 采集肉 → 羊销毁
- 羊被农民锁为目标（进入 fleeRange）→ 逃跑；不被追时静止
- 选中农民右键移动 → StopGathering() 中断采集 → 到位后恢复自动采集

## 注意事项
- 仓库名 Warriors，分支 master
- 代码注释用中文
- Library/Logs 不提交（.gitignore 已配置）
- 羊 Rigidbody2D 必须设为 Dynamic，FreezeRotation（Awake 中强制设置）
- 水的碰撞体用 EdgeCollider2D 描边（不是 Polygon，否则内外反转）
