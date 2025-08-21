# ZJHAO Bot - Minecraft Chat Bot

一个基于Minecraft Console Client (MCC)的智能聊天机器人，支持通过游戏内聊天指令控制bot行为。

## 功能特性

### 🎮 指令控制
- **权限管理**：只有授权用户才能使用bot指令
- **多种指令**：支持多种实用指令操作
- **实时响应**：在游戏内即时响应指令

### 🔐 授权用户
当前授权用户：
- `ZJHAO` (可自行添加更多用户)

## 可用指令

所有指令都需要以 `bot:` 前缀开头：

| 指令 | 功能描述 | 示例 |
|------|----------|------|
| `bot:hello` | 检查bot运行状态 | `bot:hello` |
| `bot:status` | 查看bot当前状态 | `bot:status` |
| `bot:help` | 显示帮助信息 | `bot:help` |
| `bot:respawn` | 让bot重生 | `bot:respawn` |
| `bot:exit` | 让bot退出服务器 | `bot:exit` |
| `bot:start` | 让bot重新连接服务器 | `bot:start` |

## 安装和使用

###  prerequisites
- Minecraft Console Client (MCC)
- .NET Framework

### 安装步骤
1. 将 `zjhaoBot.cs` 文件放入MCC的 `bots` 文件夹
2. 在MCC配置中加载该bot
3. 启动MCC连接服务器

### 配置说明
要添加或修改授权用户，编辑代码中的 `allowedPlayerIds` 列表：

```csharp
private readonly List<string> allowedPlayerIds = new List<string>
{
    "ZJHAO",
    "OtherPlayerName"  // 添加其他玩家ID
};
```

## 技术细节

### 消息处理机制
- 自动解析游戏聊天消息格式
- 提取玩家ID和消息内容
- 验证用户权限
- 执行相应指令

### 安全特性
- 严格的权限验证
- 防止未授权访问
- 详细的日志记录

## 开发扩展

### 添加新指令
在 `ProcessBotCommand` 方法中添加新的条件分支：

```csharp
else if (botCommand == "your_command")
{
    // 你的指令处理逻辑
    SendText("指令执行成功");
}
```

### 自定义响应
可以修改指令的响应消息和对应的 `PerformInternalCommand` 调用。

## 日志记录

所有操作都会在MCC控制台记录详细日志，包括：
- 收到的指令
- 用户权限验证结果
- 指令执行状态

## 注意事项

- 确保bot有足够的权限执行指令
- 定期检查授权用户列表
- 建议在测试服务器上先进行测试

## 支持

如有问题或建议，请提交Issue或联系开发者。

---

**免责声明**: 请遵守服务器规则，合理使用bot功能。
