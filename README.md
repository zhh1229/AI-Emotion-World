# AI Emotion World

一个从 0 到 1 构建的最小 Unity 3D + AI 情绪交互 Demo。项目最终目标是用结构化情绪结果驱动 NPC 和场景反馈，而不是制作聊天机器人或商业级游戏。

## 当前状态

当前完成阶段 4：NPC。`Environment` 场景包含基础庭院、蓝色玩家角色、键鼠移动、第三人称相机，以及一个带碰撞体的人形 NPC。玩家移动与 NPC 场景均已通过 Play Mode 实际验证。

NPC 交互、对话、AI 接入和情绪响应尚未开始实现。

## Unity 版本

- Unity Editor：`6000.3.25f1`
- 版本 revision：`e1dba0a9aba4`
- 渲染管线：Built-in Render Pipeline

请使用与 `ProjectSettings/ProjectVersion.txt` 一致的 Unity 版本打开项目，避免不必要的升级和资源重导入。

## 打开项目

1. 打开 Unity Hub。
2. 进入 `Projects`。
3. 点击 `Add`，选择本仓库根目录。
4. 确认项目使用 `6000.3.25f1` 打开。
5. 打开 `Assets/Scenes/Environment.unity`。

进入 Play Mode 后应能看到绿色地面、中央石板路以及分布在庭院中的树木、灌木和岩石。

## 操作方式

- `W`、`A`、`S`、`D`：相对相机方向移动玩家。
- 鼠标移动：旋转第三人称相机。
- 玩家使用 `CharacterController`，会受地面、树木和岩石碰撞体阻挡。
- 当前版本不包含跳跃。

## 项目结构

```text
Assets/
├── Scenes/
│   └── Environment.unity
├── Materials/
│   ├── Environment/
│   ├── NPC/
│   └── Player/
├── Prefabs/
│   ├── Environment/
│   │   ├── Plants/
│   │   ├── Rocks/
│   │   └── Trees/
│   └── NPC/
│       └── NPC_Default.prefab
├── Scripts/
│   └── Player/
│       ├── PlayerController.cs
│       └── ThirdPersonCamera.cs
├── ThirdParty/
│   └── KenneyNatureKit/
Packages/
ProjectSettings/
AGENTS.md
README.md
```

后续只在实际需要时增加 `Models`、`Animations`、`Audio` 和完整脚本子目录。

## 开发原则

- 严格按阶段开发和验证，不提前实现后续功能。
- 第一版只保留跑通 AI + Unity 情绪链路所需的最少功能。
- API Key 不写入源码、场景、Prefab 或 Git。
- Unity 生成目录和本地敏感配置不提交。
