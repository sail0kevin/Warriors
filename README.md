Eldrenn — 2D RTS Game

A classic real-time strategy game developed with Unity 2022 LTS (2D Template), featuring complete RTS core gameplay including unit control, combat, resource gathering and base building.

✨ Core Features

🎯 Unit Control System

- Flexible Selection: Single unit selection via left-click, multi-unit box selection by mouse dragging

- Intelligent Movement: Right-click ground to move units; multiple units automatically spread out for formation landing

- Selection Feedback: Selected units are highlighted in yellow with permanent health bar display

⚔️ Combat System

- Auto Target & Chase: Melee units actively detect nearby enemies and initiate combat automatically

- Animation-Driven Damage: Attack actions trigger Animator parameters, with damage calculated at precise animation frames

- Dynamic Health Bar: Floating HP bars with color gradient (Green → Yellow → Red) based on remaining health; hidden automatically when full HP

- Standard Death Logic: Dead units disable colliders and fade out transparently; buildings support instant destruction on death

🌲 Resource System

- Three Core Resources: Complete resource loop of Wood, Gold and Meat

- Auto Gathering AI: Villagers intelligently locate the nearest resource node, move and gather resources with dedicated animations and real-time resource settlement

- Dynamic Resource Nodes: Gold mines shrink progressively during mining; trees leave stumps after being harvested

🏗️ Building System

- Preview Placement Mechanic: Semi-transparent building preview follows mouse cursor; left-click to place, right-click to cancel

- Resource Verification: Automatically detect insufficient resources and restrict building placement

👥 Unit Recruitment System

- Building-Based Spawning: Open recruitment panel by clicking functional buildings, consume resources to spawn units

- Independent Cooldown: Each unit type has an exclusive recruitment cooldown timer

🤖 Diverse Unit Types

- Warrior: Melee combat unit; G key for defense stance, 1/2 keys for skill attacks

- Healer: Intelligent support unit, automatically heals the lowest-HP allied unit in range

- Villager: Civilian unit responsible for full-process automatic resource collection

- Sheep: Neutral creature with threat detection, automatically flees from enemy units

🛠 Tech Stack

- Engine: Unity 2022 LTS

- Rendering: Native 2D Pipeline (URP optional)

- Core Packages: Cinemachine, Input System, 2D Tilemap, TextMeshPro, Visual Scripting

▶️ Run Instructions

1. Open the project folder via Unity Hub

2. Load the SampleScene scene file

3. Click the Play button to start the game

⌨️ Operation Controls

In-Game Action

Key / Operation

Camera Movement

WASD

Select Units

Left-Click / Drag Box Selection

Command Unit Movement

Right-Click on Ground

Warrior Defense Stance

G

Warrior Attack Skills

1 / 2

Build Structures

Click Building Buttons

Recruit Units

Click Buildings to Open Recruitment Panel

# Eldrenn — 2D RTS 游戏

一个使用 **Unity 2022 LTS** (2D 模板) 开发的实时策略游戏。

## 游戏特色

### 单位控制
- **框选 / 点选** — 拖拽矩形框选中多个单位，或左键单击选中单个单位
- **右键移动** — 右键点击地面，单位自动寻路移动到目标点（多单位自动分散落位）
- **选中高亮** — 选中单位变黄色，并强制显示血条

### 战斗系统
- **自动索敌** — 近战单位自动检测附近敌人，追击并攻击
- **动画驱动** — 攻击触发 Animator Trigger，延迟帧结算伤害
- **血条显示** — 头顶血条，根据血量百分比变色（绿→黄→红），满血自动隐藏
- **死亡处理** — 单位死亡禁用碰撞体、半透明渐隐；建筑可设死亡直接销毁

### 资源系统
- **三种资源** — 木头、黄金、肉
- **自动采集** — 农民自动搜索最近资源点，走过去采集，动画 + 资源入账
- **资源节点** — 支持随采集缩小的金矿、采集后留下树桩

### 建造系统
- **预览放置** — 点击建筑按钮后半透明预览跟随鼠标，左键放置右键取消
- **资源消耗** — 建造前检查资源是否充足

### 招募系统
- **建筑招募** — 点击建筑打开招募面板，消耗资源生成单位
- **冷却机制** — 每个兵种独立冷却时间

### 单位类型
- **战士** — 近战输出，G 键防御，1/2 键攻击
- **治疗师** — 自动搜索并治疗友方血量最低的单位
- **农民** — 自动采集资源
- **羊** — 检测到威胁时逃跑

## 技术栈

- **引擎**: Unity 2022 LTS
- **渲染**: 2D (URP 可选)
- **包**: Cinemachine, Input System, 2D Tilemap, TextMeshPro, Visual Scripting

## 如何运行

1. 用 **Unity Hub** 打开项目文件夹
2. 打开 `SampleScene` 场景
3. 点击播放

## 操作说明

| 操作 | 按键 |
|------|------|
| 移动 | WASD |
| 选择单位 | 左键单击 / 拖拽框选 |
| 移动单位 | 右键点击地面 |
| 防御 (战士) | G |
| 攻击 (战士) | 1 / 2 |
| 建造建筑 | 点击建筑按钮 |
| 招募单位 | 点击建筑打开面板 |
