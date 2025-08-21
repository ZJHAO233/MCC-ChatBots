# 🤖 ZJHAO Bot - Minecraft智能机器人

一个基于Minecraft Console Client的高级机器人，提供智能死亡通知、精细化权限管理和丰富的指令响应功能。

## ✨ 核心特性

### 🎯 智能监控
- **⏰ 实时死亡检测** - 自动感知Bot死亡状态并发送通知
- **🛡️ 防滥用机制** - 10秒冷却时间防止频繁死亡提示
- **📊 死亡记录** - 记录并查询历史死亡信息

### 👥 权限管理
- **✅ 白名单系统** - 仅允许授权用户使用指令
- **❌ 黑名单系统** - 完全屏蔽不受欢迎用户
- **🔒 安全验证** - 严格的消息格式检查和身份验证

### 🎮 指令系统
- **📋 丰富指令集** - 包含状态查询、重生控制等实用功能
- **💬 友好交互** - 直观的反馈和帮助信息
- **⚡ 快速响应** - 即时处理用户指令

## 🚀 快速开始

### 环境要求
- [Minecraft Console Client (MCC)](https://github.com/MCCTeam/Minecraft-Console-Client)
- Minecraft Java版账号
- .Net7

### 安装步骤

1. **下载MCC并完成基础配置**
   ```bash
   # 克隆MCC仓库或下载发布版本
   git clone https://github.com/MCCTeam/Minecraft-Console-Client.git
   ```

2. **部署Bot脚本**
   - 将 `zjhaoBot.cs` 文件放入MCC的 `bots` 文件夹中

3. **启动MCC并连接服务器**
   ```bash
   # 启动MCC（具体命令取决于你的安装方式）
   .\MinecraftConsoleClient
   ```

4. **Bot自动加载**
   - 成功连接后，控制台将显示初始化消息：
   ```
   ZJHAO Bot已启动 - 死亡信息功能已启用
   ```

## ⚙️ 配置指南

### 用户权限配置
在代码中修改以下列表来自定义权限设置：

```csharp
// 白名单用户（允许使用所有指令）
private readonly List<string> allowedPlayerIds = new List<string>
{
    "ZJHAO",
    "YourUsernameHere"
    // 添加更多授权用户...
};

// 黑名单用户（完全忽略所有消息）
private readonly List<string> ignoredPlayerIds = new List<string>
{
    "YourUsernameHere"
    // 添加更多需要屏蔽的用户...
};
```

### 高级配置选项
| 配置项 | 描述 | 默认值 |
|--------|------|--------|
| 死亡提示冷却时间 | 防止频繁死亡提示的时间间隔 | 10秒 |
| 指令前缀 | 所有指令需要的前缀 | `bot:` |
| 日志级别 | 控制台输出详细程度 | 基础日志 |

## 📚 指令手册

### 基础指令
| 指令 | 功能描述 | 使用示例 |
|------|----------|----------|
| `bot:hello` | 打招呼并检查Bot状态 | `bot:hello` |
| `bot:status` | 查看Bot当前运行状态 | `bot:status` |
| `bot:help` | 显示完整的帮助菜单 | `bot:help` |

### 控制指令
| 指令 | 功能描述 | 使用示例 |
|------|----------|----------|
| `bot:respawn` | 立即重生Bot（死亡时使用） | `bot:respawn` |
| `bot:exit` | 安全退出当前服务器 | `bot:exit` |
| `bot:start` | 重新连接至服务器 | `bot:start` |

### 信息查询
| 指令 | 功能描述 | 使用示例 |
|------|----------|----------|
| `bot:deathinfo` | 查看Bot的死亡记录信息 | `bot:deathinfo` |

## 🛠️ 开发与扩展

### 添加自定义指令
在 `ProcessBotCommand` 方法中扩展新的指令：

```csharp
else if (botCommand == "your_command")
{
    // 你的自定义逻辑
    SendText("执行自定义指令");
    LogToConsole("自定义指令被执行");
}
```

### 修改响应消息
编辑代码中的消息内容来自定义Bot回应：

```csharp
SendText("你的自定义消息内容 💬");
```

### 调整时间间隔
修改死亡提示的冷却时间：

```csharp
// 将10改为你想要的秒数
if ((DateTime.Now - lastDeathTime).TotalSeconds >= 10)
```

## ❓ 常见问题

### 🤔 Bot不响应指令
1. 确认你的用户ID已添加到白名单
2. 检查指令格式是否正确（必须以`bot:`开头）
3. 验证你是否在黑名单中

### ⚠️ 频繁死亡提示
- 这是正常现象，Bot设计了10秒冷却机制防止刷屏

### 🔒 权限错误
- 确保用户ID准确无误，包括大小写和特殊字符
- 联系管理员将你的ID添加到白名单

## 📊 日志诊断

Bot的所有操作都会在MCC控制台中记录，常见日志消息：

| 日志消息 | 含义 |
|----------|------|
| `收到授权用户 [ID] 的bot指令` | 成功处理授权指令 |
| `忽略未授权用户 [ID] 的bot指令` | 拒绝未授权用户 |
| `忽略黑名单用户 [ID] 的消息` | 屏蔽黑名单用户 |
| `Bot死亡，发送死亡信息...` | 处理死亡事件 |

## 🤝 贡献指南

欢迎各种形式的贡献！

### 开发规范
- 遵循现有的代码风格
- 添加清晰的注释说明
- 更新相应的文档
- 测试你的修改

## 🏆 致谢

- 感谢 [MCC Team](https://github.com/MCCTeam) 提供的强大框架
- 感谢所有贡献者和测试者
- 特别感谢用户们的反馈和建议

---

⭐ **如果这个项目对你有帮助，请给我一个Star！**

---

**注意**: 使用本Bot请遵守服务器规则，尊重其他玩家体验。不当使用可能导致服务器封禁。
