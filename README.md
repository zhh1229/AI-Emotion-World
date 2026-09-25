# AI Emotion World

一个从 0 到 1 构建的最小 Unity 3D + AI 情绪交互 Demo。项目最终目标是用结构化情绪结果驱动 NPC 和场景反馈，而不是制作聊天机器人或商业级游戏。

## 当前状态

当前完成阶段 2：最小 3D 场景。`Environment` 场景已包含地面、道路、树木、灌木、岩石、碰撞体、基础灯光和相机，并已在 Play Mode 中运行验证。

玩家移动、NPC、对话、AI 接入和情绪响应尚未开始实现。

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

## 项目结构

```text
Assets/
├── Scenes/
│   └── Environment.unity
├── Materials/
│   └── Environment/
├── Prefabs/
│   └── Environment/
│       ├── Plants/
│       ├── Rocks/
│       └── Trees/
├── ThirdParty/
│   └── KenneyNatureKit/
└── Scripts/
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
