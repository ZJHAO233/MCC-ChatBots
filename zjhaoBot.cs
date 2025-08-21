//MCCScript 1.0

MCC.LoadBot(new SystemMessageQuitBot());

//MCCScript Extensions
class SystemMessageQuitBot : ChatBot
{
    // 允许控制bot的玩家ID白名单
    private readonly List<string> allowedPlayerIds = new List<string>
    {
        "ZJHAO",
        "_xiaorongyu(小鱼)",
        "_xiaorongyu",
        "FurryFanson"
    };

    private readonly List<string> ignoredPlayerIds = new List<string>
    {
        "Kagantuya"
    };

    private DateTime lastDeathTime = DateTime.MinValue;
    private List<string> homeList = new List<string>();
    private bool isCollectingHomes = false;
    private DateTime homeRequestTime;

    // 添加home列表响应收集的标志
    private bool isProcessingHomeList = false;

    public override void Initialize()
    {
        LogToConsole("ZJHAO Bot已启动 - 死亡信息功能已启用");
    }

    public override void OnDeath()
    {
        if (ShouldProcessDeath())
        {
            HandleDeath();
        }
    }

    public override void GetText(string text)
    {
        string rawText = GetVerbatim(text);

        // 首先检查是否是home列表的系统响应
        if (isProcessingHomeList && IsHomeListSystemResponse(rawText))
        {
            ProcessHomeListSystemResponse(rawText);
            return;
        }

        // 然后检查是否正在收集home列表信息
        if (isCollectingHomes)
        {
            ProcessHomeListResponse(rawText);
            return;
        }

        // 最后处理普通玩家消息
        if (TryParsePlayerMessage(rawText, out string playerId, out string messageContent))
        {
            ProcessPlayerMessage(playerId, messageContent);
        }
    }

    /// <summary>
    /// 检查是否是home列表的系统响应（特殊格式）
    /// </summary>
    private bool IsHomeListSystemResponse(string rawText)
    {
        // 系统home列表响应通常包含特定格式
        return rawText.Contains("的家:") && rawText.Contains("共") && rawText.Contains("个)");
    }

    /// <summary>
    /// 处理home列表的系统响应
    /// </summary>
    private void ProcessHomeListSystemResponse(string rawText)
    {
        LogToConsole($"收到home列表系统响应: {rawText}");

        try
        {
            // 解析home列表格式：Kagantuya 的家: (1-6 共6个)  [home1] • [home2] • ...
            int colonIndex = rawText.IndexOf(':');
            if (colonIndex > 0 && colonIndex < rawText.Length - 1)
            {
                string homesPart = rawText.Substring(colonIndex + 1).Trim();

                // 提取home名称（在方括号中的内容）
                int startIndex = 0;
                while (startIndex < homesPart.Length)
                {
                    int openBracket = homesPart.IndexOf('[', startIndex);
                    if (openBracket == -1) break;

                    int closeBracket = homesPart.IndexOf(']', openBracket + 1);
                    if (closeBracket == -1) break;

                    string homeName = homesPart.Substring(openBracket + 1, closeBracket - openBracket - 1);
                    if (!string.IsNullOrWhiteSpace(homeName))
                    {
                        homeList.Add(homeName.Trim());
                    }

                    startIndex = closeBracket + 1;
                }
            }

            // 完成home列表收集
            FinishHomeListCollection();
        }
        catch (Exception ex)
        {
            LogToConsole($"解析home列表时出错: {ex.Message}");
            HandleHomeListError();
        }
    }

    private bool ShouldProcessDeath()
    {
        return (DateTime.Now - lastDeathTime).TotalSeconds >= 10;
    }

    private void HandleDeath()
    {
        LogToConsole("Bot死亡，发送死亡信息...");
        SendText("💀 Bot不幸死亡了！需要救援或自动重生吗？");
        SendText("使用指令: bot:respawn 让我重生");
        lastDeathTime = DateTime.Now;
    }

    private bool TryParsePlayerMessage(string rawText, out string playerId, out string messageContent)
    {
        playerId = null;
        messageContent = null;

        if (!rawText.Contains(":") || !rawText.Contains("]"))
            return false;

        int bracketIndex = rawText.IndexOf(']');
        int colonIndex = rawText.IndexOf(':');

        if (bracketIndex <= 0 || colonIndex <= bracketIndex || colonIndex >= rawText.Length - 1)
            return false;

        playerId = rawText.Substring(bracketIndex + 1, colonIndex - bracketIndex - 1).Trim();
        messageContent = rawText.Substring(colonIndex + 1).Trim();

        return true;
    }

    private void ProcessPlayerMessage(string playerId, string messageContent)
    {
        if (ignoredPlayerIds.Contains(playerId))
        {
            LogToConsole($"忽略黑名单用户 {playerId} 的消息");
            return;
        }

        if (messageContent.StartsWith("bot:"))
        {
            HandleBotCommand(playerId, messageContent);
        }
    }

    private void HandleBotCommand(string playerId, string command)
    {
        if (!allowedPlayerIds.Contains(playerId))
        {
            LogToConsole($"忽略未授权用户 {playerId} 的bot指令");
            SendText($"抱歉 {playerId}，你没有权限使用此bot");
            return;
        }

        LogToConsole($"收到授权用户 {playerId} 的bot指令: {command}");
        ProcessBotCommand(command.Substring(4).Trim(), playerId);
    }

    private void ProcessBotCommand(string botCommand, string playerId)
    {
        var commandHandlers = new Dictionary<string, Action>
        {
            ["hello"] = () => SendText($"Hello {playerId}! Bot运行正常"),
            ["status"] = () => ShowStatus(),
            ["help"] = () => ShowHelp(),
            ["exit"] = () => ExitServer(playerId),
            ["start"] = () => Reconnect(playerId),
            ["respawn"] = () => Respawn(playerId),
            ["deathinfo"] = () => ShowDeathInfo(),
            ["players"] = () => ShowOnlinePlayers(),
            ["homes"] = () => ShowHomes()
        };

        if (commandHandlers.TryGetValue(botCommand, out Action handler))
        {
            handler();
        }
        else
        {
            HandleUnknownCommand(playerId, botCommand);
        }
    }

    private void ShowStatus()
    {
        Location currentLocation = GetCurrentLocation();

        // 简化坐标输出，只保留整数部分
        string coordinates = $"X: {(int)currentLocation.X}, Y: {(int)currentLocation.Y}, Z: {(int)currentLocation.Z}";

        SendText("状态: 在线挂机中");
        SendText($"当前位置: {coordinates}");
    }

    private void ShowHelp()
    {
        SendText("🤖 ZJHAO Bot 帮助文档:");
        SendText("📖 详细使用说明请查看: https://docs.qq.com/doc/DSnVGY1hEdEtiTE9P#");
        SendText("💡 所有指令都需要以 'bot:' 开头");
    }

    private void ExitServer(string playerId)
    {
        LogToConsole($"授权用户 {playerId} 通过bot指令请求退服...");
        SendText("正在退服…");
        PerformInternalCommand("exit");
    }

    private void Reconnect(string playerId)
    {
        LogToConsole($"授权用户 {playerId} 通过bot指令请求重连...");
        SendText("正在重连…");
        PerformInternalCommand("reco");
    }

    private void Respawn(string playerId)
    {
        LogToConsole($"授权用户 {playerId} 通过bot指令请求重生...");
        SendText("正在重生…");
        PerformInternalCommand("respawn");
    }

    private void ShowDeathInfo()
    {
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

    private void HandleUnknownCommand(string playerId, string botCommand)
    {
        SendText($"用户:{playerId},发送了其他指令: {botCommand}");
        SendText(botCommand);
    }

    private void ProcessHomeListResponse(string rawText)
    {
        if (IsHomeListTimeout())
        {
            HandleHomeListTimeout();
            return;
        }

        if (IsHomeEntry(rawText))
        {
            homeList.Add(rawText.Trim());
            LogToConsole($"收集到home信息: {rawText}");
        }
        else if (IsHomeListEnd(rawText))
        {
            FinishHomeListCollection();
        }
        else if (IsHomeListError(rawText))
        {
            HandleHomeListError();
        }
    }

    private bool IsHomeListTimeout()
    {
        return (DateTime.Now - homeRequestTime).TotalSeconds > 5;
    }

    private void HandleHomeListTimeout()
    {
        isCollectingHomes = false;
        isProcessingHomeList = false;
        SendText("❌ 获取home列表超时，请重试");
        homeList.Clear();
    }

    private bool IsHomeEntry(string rawText)
    {
        return rawText.Contains(":") && (rawText.Contains("home") || rawText.Contains("Home"));
    }

    private bool IsHomeListEnd(string rawText)
    {
        return string.IsNullOrWhiteSpace(rawText) ||
               rawText.Contains("没有设置") ||
               rawText.Contains("No homes") ||
               rawText.Contains("home list is empty");
    }

    private bool IsHomeListError(string rawText)
    {
        return rawText.Contains("未知命令") ||
               rawText.Contains("Unknown command") ||
               rawText.Contains("没有权限");
    }

    private void FinishHomeListCollection()
    {
        isCollectingHomes = false;
        isProcessingHomeList = false;

        if (homeList.Count > 0)
        {
            SendHomeList();
        }
        else
        {
            SendText("❌ 没有设置任何home");
        }

        homeList.Clear();
    }

    /// <summary>
    /// 发送home列表（简化为一到两行输出）
    /// </summary>
    private void SendHomeList()
    {
        // 将所有home名称用逗号分隔连接成一行
        string homeListString = string.Join(", ", homeList);

        // 如果home列表太长，分割成两行
        if (homeListString.Length > 100)
        {
            SendText($"🏠 Home列表({homeList.Count}个): {homeListString.Substring(0, 100)}...");
            SendText(homeListString.Substring(100));
        }
        else
        {
            SendText($"🏠 Home列表({homeList.Count}个): {homeListString}");
        }
    }

    private void HandleHomeListError()
    {
        isCollectingHomes = false;
        isProcessingHomeList = false;
        SendText("❌ 无法获取home列表：没有权限或命令不存在");
        homeList.Clear();
    }

    private void ShowOnlinePlayers()
    {
        try
        {
            string[] players = GetOnlinePlayers();

            if (players == null || players.Length == 0)
            {
                SendText("❌ 无法获取在线玩家列表或服务器为空");
                return;
            }

            SendPlayerList(players);
        }
        catch (Exception ex)
        {
            LogToConsole("获取在线玩家列表时出错: " + ex.Message);
            SendText("❌ 获取在线玩家列表时出错");
        }
    }

    private void SendPlayerList(string[] players)
    {
        SendText("🟢 服务器在线玩家:");

        string playerList = string.Join(", ", players);

        if (playerList.Length > 100)
        {
            SendLongMessage(playerList);
        }
        else
        {
            SendText(playerList);
        }

        SendText($"总计: {players.Length} 名玩家在线");
    }

    private void SendLongMessage(string message)
    {
        int maxLength = 100;
        for (int i = 0; i < message.Length; i += maxLength)
        {
            int length = Math.Min(maxLength, message.Length - i);
            SendText(message.Substring(i, length));
        }
    }

    private void ShowHomes()
    {
        try
        {
            homeList.Clear();
            isCollectingHomes = true;
            isProcessingHomeList = true;
            homeRequestTime = DateTime.Now;

            SendText("/homelist");
            LogToConsole("已发送 /homelist 命令，等待服务器响应...");
            SendText("📋 正在获取home列表...");

            SetupHomeListTimeoutCheck();
        }
        catch (Exception ex)
        {
            LogToConsole("获取home列表时出错: " + ex.Message);
            SendText("❌ 获取home列表时出错");
            isCollectingHomes = false;
            isProcessingHomeList = false;
        }
    }

    private void SetupHomeListTimeoutCheck()
    {
        System.Threading.Tasks.Task.Delay(5000).ContinueWith(t =>
        {
            if (isCollectingHomes || isProcessingHomeList)
            {
                isCollectingHomes = false;
                isProcessingHomeList = false;
                SendText("❌ 获取home列表超时，请重试");
                homeList.Clear();
            }
        });
    }
}