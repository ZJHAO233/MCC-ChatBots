//MCCScript 1.0

MCC.LoadBot(new SystemMessageQuitBot());

//MCCScript Extensions
class SystemMessageQuitBot : ChatBot
{
    // 定义允许控制bot的玩家ID列表
    // 只有在这个列表中的用户发送的指令才会被处理
    private readonly List<string> allowedPlayerIds = new List<string>
    {
        "ZJHAO",
        "_xiaorongyu(小鱼)",
        "_xiaorongyu",
        "FurryFanson"
        // 可以继续添加其他允许的ID
    };

    // 定义忽略的黑名单玩家ID列表
    // 在这个列表中的用户发送的消息将被完全忽略
    private readonly List<string> ignoredPlayerIds = new List<string>
    {
        "Kagantuya"
        // 可以继续添加其他要忽略的ID
    };

    // 记录上一次死亡时间，防止频繁发送死亡信息
    private DateTime lastDeathTime = DateTime.MinValue;

    // 用于存储home列表
    private List<string> homeList = new List<string>();
    private bool isCollectingHomes = false;
    private DateTime homeRequestTime;

    public override void Initialize()
    {
        LogToConsole("ZJHAO Bot已启动 - 死亡信息功能已启用");
    }

    // 处理死亡事件
    public override void OnDeath()
    {
        // 检查距离上次死亡是否至少10秒，防止频繁触发
        if ((DateTime.Now - lastDeathTime).TotalSeconds >= 10)
        {
            LogToConsole("Bot死亡，发送死亡信息...");
            SendText("💀 Bot不幸死亡了！需要救援或自动重生吗？");
            SendText("使用指令: bot:respawn 让我重生");

            // 更新最后死亡时间
            lastDeathTime = DateTime.Now;
        }
        else
        {
            LogToConsole("检测到死亡，但距离上次死亡时间太近，忽略本次提示");
        }
    }

    // 处理所有收到的文本消息
    public override void GetText(string text)
    {
        // 获取原始文本（去除格式代码，如颜色代码等）
        string rawText = GetVerbatim(text);

        // 检查是否正在收集home列表信息
        if (isCollectingHomes)
        {
            ProcessHomeListResponse(rawText);
        }

        // 检查消息是否符合系统消息格式（包含]和:）
        if (rawText.Contains(":") && rawText.Contains("]"))
        {
            // 查找右括号和冒号的位置
            int bracketIndex = rawText.IndexOf(']');
            int colonIndex = rawText.IndexOf(':');

            // 确保位置有效：括号在冒号之前，且冒号不在字符串末尾
            if (bracketIndex > 0 && colonIndex > bracketIndex && colonIndex < rawText.Length - 1)
            {
                // 提取玩家ID部分（位于]和:之间的内容）
                // 例如：[粉丝服] ZJHAO:hello → 提取出"ZJHAO"
                string playerId = rawText.Substring(bracketIndex + 1, colonIndex - bracketIndex - 1).Trim();

                // 检查玩家是否在黑名单中
                if (ignoredPlayerIds.Contains(playerId))
                {
                    LogToConsole($"忽略黑名单用户 {playerId} 的消息");
                    return; // 直接返回，不处理任何来自黑名单用户的消息
                }

                // 提取消息内容部分（冒号后面的所有内容）
                // 例如：[粉丝服] ZJHAO:hello → 提取出"hello"
                string messageContent = rawText.Substring(colonIndex + 1).Trim();

                // 只处理以"bot:"开头的消息
                if (messageContent.StartsWith("bot:"))
                {
                    // 检查玩家ID是否在允许的授权列表中
                    if (allowedPlayerIds.Contains(playerId))
                    {
                        // 如果是授权用户，记录日志并处理指令
                        LogToConsole($"收到授权用户 {playerId} 的bot指令: {messageContent}");

                        // 调用方法处理授权用户的bot指令
                        ProcessBotCommand(messageContent, playerId);
                    }
                    else
                    {
                        // 如果是未授权用户，记录日志但忽略指令
                        LogToConsole($"忽略未授权用户 {playerId} 的bot指令");
                        // 发送权限提示消息
                        SendText($"抱歉 {playerId}，你没有权限使用此bot");
                    }
                }
                // 如果不是bot指令，直接忽略不处理
            }
        }
    }

    // 处理home列表响应
    private void ProcessHomeListResponse(string rawText)
    {
        // 检查是否超时（5秒内没有收到home列表响应）
        if ((DateTime.Now - homeRequestTime).TotalSeconds > 5)
        {
            isCollectingHomes = false;
            SendText("❌ 获取home列表超时");
            return;
        }

        // 检查是否是home列表行（通常包含home名称和坐标）
        if (rawText.Contains(":") && (rawText.Contains("home") || rawText.Contains("Home")))
        {
            // 添加到home列表
            homeList.Add(rawText.Trim());
        }
        // 检查是否是home列表结束的标志（如"没有设置home"或空行）
        else if (string.IsNullOrWhiteSpace(rawText) || rawText.Contains("没有设置") || rawText.Contains("No homes"))
        {
            isCollectingHomes = false;

            // 显示收集到的home列表
            if (homeList.Count > 0)
            {
                SendText("🏠 Home列表:");
                foreach (string home in homeList)
                {
                    SendText(home);
                }
                SendText("总计: " + homeList.Count + " 个home");
            }
            else
            {
                SendText("❌ 没有设置任何home");
            }

            homeList.Clear();
        }
    }

    // 专门处理bot指令的私有方法
    private void ProcessBotCommand(string command, string playerId)
    {
        // 提取bot指令的具体内容（去掉"bot:"前缀）
        string botCommand = command.Substring(4).Trim();

        // 根据不同的bot指令做出响应
        if (botCommand == "hello")
        {
            SendText($"Hello {playerId}! Bot运行正常");
        }
        else if (botCommand == "status")
        {
            // 获取当前坐标
            Location currentLocation = GetCurrentLocation();
            string coordinates = $"X: {currentLocation.X}, Y: {currentLocation.Y}, Z: {currentLocation.Z}";

            SendText("状态: 在线挂机中");
            SendText($"当前位置: {coordinates}");
        }
        else if (botCommand == "help")
        {
            SendText("🤖 ZJHAO Bot 帮助文档:");
            SendText("📖 详细使用说明请查看: https://docs.qq.com/doc/DSnVGY1hEdEtiTE9P#");
            SendText("💡 所有指令都需要以 'bot:' 开头");
        }
        else if (botCommand == "exit")
        {
            LogToConsole($"授权用户 {playerId} 通过bot指令请求退服...");
            SendText("正在退服…");
            PerformInternalCommand("exit");  // 执行退出服务器命令
        }
        else if (botCommand == "start")
        {
            LogToConsole($"授权用户 {playerId} 通过bot指令请求重连...");
            SendText("正在重连…");
            PerformInternalCommand("reco");  // 执行重连命令
        }
        else if (botCommand == "respawn")
        {
            LogToConsole($"授权用户 {playerId} 通过bot指令请求重生...");
            SendText("正在重生…");
            PerformInternalCommand("respawn");  // 执行重生命令
        }
        else if (botCommand == "deathinfo")
        {
            // 显示死亡相关信息
            if (lastDeathTime != DateTime.MinValue)
            {
                TimeSpan timeSinceDeath = DateTime.Now - lastDeathTime;
                SendText($"💀 上次死亡时间: {lastDeathTime:HH:mm:ss} ({timeSinceDeath.TotalMinutes:F1}分钟前)");
            }
            else
            {
                SendText("✅ Bot尚未死亡过");
            }
        }
        else if (botCommand == "players")
        {
            // 显示服务器在线玩家列表
            ShowOnlinePlayers();
        }
        else if (botCommand == "homes")
        {
            // 显示home列表
            ShowHomes();
        }
        else
        {
            // 只有当前面所有条件都不匹配时，才执行这里的代码
            // 处理未知的bot指令 - 显示用户ID和未知指令
            SendText($"用户:{playerId},发送了其他指令: {botCommand}");
            SendText(botCommand);
            // SendText($"💡 请输入 bot:help 查看可用指令列表");
        }
    }

    // 显示服务器在线玩家列表
    private void ShowOnlinePlayers()
    {
        try
        {
            // 获取当前在线玩家列表 - GetOnlinePlayers() 返回 string[]
            string[] players = GetOnlinePlayers();

            if (players != null && players.Length > 0)
            {
                SendText("🟢 服务器在线玩家:");

                // 将玩家列表连接成字符串
                string playerList = string.Join(", ", players);

                // 如果玩家列表太长，分割成多个消息
                if (playerList.Length > 100)
                {
                    int maxLength = 100;
                    for (int i = 0; i < playerList.Length; i += maxLength)
                    {
                        int length = Math.Min(maxLength, playerList.Length - i);
                        SendText(playerList.Substring(i, length));
                    }
                }
                else
                {
                    SendText(playerList);
                }

                // 使用字符串连接而不是插值字符串以避免C#版本兼容性问题
                SendText("总计: " + players.Length + " 名玩家在线");
            }
            else
            {
                SendText("❌ 无法获取在线玩家列表或服务器为空");
            }
        }
        catch (Exception ex)
        {
            LogToConsole("获取在线玩家列表时出错: " + ex.Message);
            SendText("❌ 获取在线玩家列表时出错");
        }
    }

    // 显示home列表
    private void ShowHomes()
    {
        try
        {
            // 重置home列表和状态
            homeList.Clear();
            isCollectingHomes = true;
            homeRequestTime = DateTime.Now;

            // 发送指令获取home列表
            SendText("/homelist");

            SendText("📋 正在获取home列表...");
        }
        catch (Exception ex)
        {
            LogToConsole("获取home列表时出错: " + ex.Message);
            SendText("❌ 获取home列表时出错");
            isCollectingHomes = false;
        }
    }
}
