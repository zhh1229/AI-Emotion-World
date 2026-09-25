# AI Emotion World

一个从 0 到 1 构建的最小 Unity 3D + AI 情绪交互 Demo。项目最终目标是用结构化情绪结果驱动 NPC 和场景反馈，而不是制作聊天机器人或商业级游戏。

## 项目概览

MVP 包含：

- 一个可运行的小型 3D 庭院。
- 键鼠控制的玩家和第三人称相机。
- 可接近并交互的人形 NPC。
- 支持中文的对话输入与回复 UI。
- DeepSeek 文本请求和结构化情绪 JSON 响应。
- `happy`、`sad`、`angry`、`calm`、`neutral` 五种情绪。
- NPC 姿态、颜色和场景灯光、背景、地面反馈。

## 当前状态

当前完成阶段 14：README 完善。项目已通过完整端到端验证，并整理出可复现的安装、配置和运行说明。

后续进入 Git 提交整理和最终打包阶段。

## 环境要求

- Windows PC，当前项目在 Windows 上完成验证。
- Unity Hub 和 `Unity 6000.3.25f1`。
- 可访问 DeepSeek 官方 API 的网络环境。
- 一个保存在本机且不提交到 Git 的 DeepSeek API Key。
- 第一版不需要 Python、数据库或自建服务器。

## Unity 版本

- Unity Editor：`6000.3.25f1`
- 版本 revision：`e1dba0a9aba4`
- 渲染管线：Built-in Render Pipeline

请使用与 `ProjectSettings/ProjectVersion.txt` 一致的 Unity 版本打开项目，避免不必要的升级和资源重导入。

## 快速开始

1. 打开 Unity Hub。
2. 进入 `Projects`。
3. 点击 `Add`，选择本仓库根目录。
4. 确认项目使用 `6000.3.25f1` 打开。
5. 打开 `Assets/Scenes/Environment.unity`。
6. 按下方“DeepSeek 配置”配置本机 API Key。
7. 点击 Play，靠近 NPC 并按 `E` 开始对话。

进入 Play Mode 后应能看到绿色地面、中央石板路以及分布在庭院中的树木、灌木和岩石。

## 操作方式

- `W`、`A`、`S`、`D`：相对相机方向移动玩家。
- 鼠标移动：旋转第三人称相机。
- 靠近 NPC 至 `2.5 m` 内按 `E`：打开对话面板并使 NPC 转向玩家。
- 点击输入框并输入文本，点击 `Send` 或按回车：发送 DeepSeek 请求并显示回复。
- `Esc`：关闭对话面板。
- 输入框支持中文输入和中文 AI 回复显示。
- 对话面板打开期间玩家仍可移动；输入框获得焦点时，`W`、`A`、`S`、`D` 会同时参与文本输入和移动。
- 玩家使用 `CharacterController`，会受地面、树木和岩石碰撞体阻挡。
- 当前版本不包含跳跃。

## 整体测试结果

- 玩家移动：前进 `6` 米并保持落地；对话面板打开时仍可移动。
- NPC 交互：`5.61 m` 不触发，`1.6 m` 正确选中 `Courtyard Visitor`。
- 对话 UI：打开、输入、发送、回复和 `Esc` 关闭均通过。
- 中文输入：输入框生成中文字形并显示中文回复。
- DeepSeek：真实请求成功解析为结构化情绪结果。
- 情绪反馈：真实 `happy`、`sad`、`angry` 请求分别驱动不同 NPC 姿态和场景颜色。
- 错误恢复：一次瞬时网络传输失败会显示错误，恢复后重新发送成功。
- 非法响应：缺字段、未知情绪、强度越界、非法 JSON 和超时均有明确错误。
- 安全：仓库内未发现 API Key，本地 DeepSeek 配置位于仓库外。

## 代码整理

- AI 测试专用配置接口只在 Unity Editor 中编译，不进入正式构建。
- AI 请求失败时清除旧结构化结果，避免调用方读取过期情绪。
- 中性状态恢复原始灯光、雾开关和雾密度。
- 移除 NPC 交互时重复的 Console 输出。
- 重新编译并回归验证玩家移动、中文输入、对话和情绪反馈。

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

## 场景情绪反馈

| 情绪 | 场景表现 |
|---|---|
| `happy` | 暖金色高亮灯光、明亮蓝天、金黄色地面 |
| `sad` | 冷蓝低亮灯光、深蓝环境色、低饱和地面和轻雾 |
| `angry` | 红色高亮灯光、暗红背景、红棕地面和轻雾 |
| `calm` | 青绿色柔和灯光、清爽背景和明亮草地 |
| `neutral` | 恢复原始灯光、背景和地面颜色 |

情绪事件到达时，NPC 姿态和场景反馈会在同一帧立即改变；`Update` 负责后续平滑动画。

## 中文字体

对话 UI 使用 Noto Sans SC 作为 TMP 动态中文字体。输入文字变化和 AI 回复显示前会按需加入所需字形。

- 字体来源：<https://github.com/notofonts/noto-cjk>
- 字体文件：`NotoSansSC-Regular.otf`
- 许可证：SIL Open Font License 1.1
- 项目位置：`Assets/ThirdParty/NotoSansSC`

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
│   ├── Core/
│   │   └── SceneEmotionFeedback.cs
│   ├── NPC/
│   │   ├── NpcEmotionState.cs
│   │   ├── NpcEmotionReaction.cs
│   │   └── NpcInteractable.cs
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerInteractor.cs
│   │   └── ThirdPersonCamera.cs
│   └── UI/
│       ├── ChineseFontAssetProvider.cs
│       └── DialogueUI.cs
├── ThirdParty/
│   ├── KenneyNatureKit/
│   └── NotoSansSC/
Packages/
ProjectSettings/
AGENTS.md
README.md
```

后续只在实际需要时增加 `Models`、`Animations`、`Audio` 等目录。

## 主要依赖

- `com.unity.ugui 2.0.0`：Canvas UI、输入框和 TextMeshPro。
- `com.unity.pipeline 0.7.0-exp.1`：Codex/AI Agent 对本机 Unity Editor 的实时控制，不影响游戏运行逻辑。
- Unity Built-in Render Pipeline：沿用原项目渲染管线，不额外引入 URP/HDRP。
- `Newtonsoft.Json`：通过 Unity Package Manager 依赖提供结构化 AI 响应解析。

## 故障排查

### 提示缺少 API Key

确认 `DEEPSEEK_API_KEY` 已设置，或 `deepseek-config.json` 位于：

```text
%USERPROFILE%\AppData\LocalLow\DefaultCompany\AI Emotion World\
```

修改环境变量后需要重新启动 Unity Editor。

### API 返回 401

通常表示 Key 无效、过期或复制时包含多余空格。请只在本机更新配置，不要发送给其他人。

### 请求超时或瞬时失败

界面会显示错误且恢复输入。检查网络后重新发送即可，不需要重启 Play Mode。

### 中文无法显示

确认 `Assets/ThirdParty/NotoSansSC/NotoSansSC-Regular.otf` 和 `OFL.txt` 存在。UI 会在运行时创建 TMP 动态中文字体并按需补充字形。

### 输入框有焦点时移动

这是当前设计。`W`、`A`、`S`、`D` 会同时参与文本输入和玩家移动，避免对话期间锁定角色。

## 已知限制

- NPC 是基础人形占位模型，不包含骨骼动画或高质量面部表现。
- 对话为单轮请求，暂不保存长期上下文和长期记忆。
- 当前只验证 Windows Editor，尚未进行最终平台打包。
- 未包含多人联机、账号、数据库、语音和复杂场景系统。

## 第三方资源与许可证

- Kenney Nature Kit：CC0 1.0，许可证位于 `Assets/ThirdParty/KenneyNatureKit/License.txt`。
- Noto Sans SC：SIL Open Font License 1.1，来源和许可证位于 `Assets/ThirdParty/NotoSansSC`。
- API Key 和本机 DeepSeek 配置文件不属于项目资源，禁止提交到仓库。

## 开发原则

- 严格按阶段开发和验证，不提前实现后续功能。
- 第一版只保留跑通 AI + Unity 情绪链路所需的最少功能。
- API Key 不写入源码、场景、Prefab 或 Git。
- Unity 生成目录和本地敏感配置不提交。
