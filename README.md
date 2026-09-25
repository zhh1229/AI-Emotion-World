# AI Emotion World

一个从 0 到 1 构建的最小 Unity 3D + AI 情绪交互 Demo。项目最终目标是用结构化情绪结果驱动 NPC 和场景反馈，而不是制作聊天机器人或商业级游戏。

## 当前状态

当前完成阶段 10：NPC 情绪反应。`NpcEmotionState` 的五种情绪会触发不同颜色、姿态和动作，对话回复与 NPC 表现会同步更新。

玩家移动、NPC、交互、对话 UI、模拟 API 错误处理、真实 DeepSeek 请求、结构化情绪解析、NPC 情绪状态同步和五种 NPC 情绪表现均已通过实际运行验证。场景级情绪反馈尚未开始实现。

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
- 靠近 NPC 至 `2.5 m` 内按 `E`：打开对话面板并使 NPC 转向玩家。
- 点击输入框并输入文本，点击 `Send` 或按回车：显示 NPC 占位回复。
- `Esc`：关闭对话面板并恢复玩家移动。
- 玩家使用 `CharacterController`，会受地面、树木和岩石碰撞体阻挡。
- 当前版本不包含跳跃。

## DeepSeek 配置

真实 API Key 禁止写入仓库。程序按以下优先顺序读取配置：

1. 环境变量：`DEEPSEEK_API_KEY`、`DEEPSEEK_API_URL`、`DEEPSEEK_MODEL`。
2. 本机用户配置：

```text
%USERPROFILE%\AppData\LocalLow\DefaultCompany\AI Emotion World\deepseek-config.json
```

本地配置模板：

```json
{
  "apiKey": "在本机填写，不要提交",
  "apiUrl": "https://api.deepseek.com/chat/completions",
  "model": "deepseek-v4-flash"
}
```

默认接口为 `https://api.deepseek.com/chat/completions`，默认模型为 `deepseek-v4-flash`。

AI 必须返回以下结构：

```json
{
  "emotion": "happy",
  "intensity": 0.8,
  "reply": "嗨！今天天气真好，很高兴见到你！"
}
```

- `emotion` 只能是 `happy`、`sad`、`angry`、`calm`、`neutral`。
- `intensity` 必须是 `0` 到 `1` 之间的数字。
- `reply` 必须是非空字符串。
- 缺字段、未知情绪、强度越界、非法 JSON 和非 2xx 响应都会显示明确错误。

## 情绪链路

```text
玩家输入
-> DeepSeekChatClient 提取原始回复
-> EmotionConversationService 严格解析
-> EmotionResult
-> NpcEmotionState
-> DialogueUI 显示 reply
```

`EmotionConversationService` 负责请求编排和解析，`NpcEmotionState` 保存当前情绪与强度。非法响应不会修改 NPC 当前状态。

## NPC 情绪表现

| 情绪 | 视觉表现 |
|---|---|
| `happy` | 金色躯干、抬头上扬、双臂展开 |
| `sad` | 蓝色躯干、低头前倾、手臂下垂 |
| `angry` | 红色躯干、双臂张开、快速抖动 |
| `calm` | 绿色躯干、自然呼吸、身体平稳 |
| `neutral` | 原始颜色和标准站姿 |

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
│   ├── AI/
│   │   ├── ChatCompletionData.cs
│   │   ├── DeepSeekApiSettings.cs
│   │   ├── DeepSeekChatClient.cs
│   │   ├── DeepSeekEmotionResponseParser.cs
│   │   ├── EmotionConversationService.cs
│   │   └── EmotionResult.cs
│   ├── NPC/
│   │   ├── NpcEmotionState.cs
│   │   ├── NpcEmotionReaction.cs
│   │   └── NpcInteractable.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerInteractor.cs
│   │   └── ThirdPersonCamera.cs
│   └── UI/
│       └── DialogueUI.cs
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
