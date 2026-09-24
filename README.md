# AI Emotion World

一个从 0 到 1 构建的最小 Unity 3D + AI 情绪交互 Demo。项目最终目标是用结构化情绪结果驱动 NPC 和场景反馈，而不是制作聊天机器人或商业级游戏。

## 当前状态

当前处于阶段 1：Unity 项目初始化。玩家、NPC、场景、对话、AI 接入和情绪响应尚未开始实现。

## Unity 版本

- Unity Editor：`2022.3.62f3`
- 版本 revision：`96770f904ca7`
- 渲染管线：Unity 2022.3 默认模板

请使用与 `ProjectSettings/ProjectVersion.txt` 一致的 Unity 版本打开项目，避免不必要的升级和资源重导入。

## 打开项目

1. 打开 Unity Hub。
2. 进入 `Projects`。
3. 点击 `Add`，选择本仓库根目录。
4. 确认项目使用 `2022.3.62f3` 打开。

本阶段只验证工程可以打开和编译，不创建玩法场景。

## 项目结构

```text
Assets/
├── Scenes/
└── Scripts/
Packages/
ProjectSettings/
AGENTS.md
README.md
```

后续只在实际需要时增加 `Prefabs`、`Models`、`Materials`、`Animations`、`Audio` 和完整脚本子目录。

## 开发原则

- 严格按阶段开发和验证，不提前实现后续功能。
- 第一版只保留跑通 AI + Unity 情绪链路所需的最少功能。
- API Key 不写入源码、场景、Prefab 或 Git。
- Unity 生成目录和本地敏感配置不提交。
