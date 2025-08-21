//MCCScript 1.0

MCC.LoadBot(new SystemMessageQuitBot());

//MCCScript Extensions
class SystemMessageQuitBot : ChatBot
{
    // 定义允许控制bot的玩家ID列表
    // 只有在这个列表中的用户发送的指令才会被处理
    private readonly List<string> allowedPlayerIds = new List<string>
    {
        "ZJHAO"
        // 可以继续添加其他允许的ID
    };

    // 处理所有收到的文本消息
    public override void GetText(string text)
    {
        // 获取原始文本（去除格式代码，如颜色代码等）
        string rawText = GetVerbatim(text);

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
            SendText("状态: 在线挂机中");
        }
        else if (botCommand == "help")
        {
            SendText("可用bot指令: bot:hello, bot:status, bot:help, bot:respawn, bot:exit, bot:start");
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
        else
        {
            // 处理未知的bot指令
            SendText($"授权用户：{playerId}发送: {botCommand}");
            SendText(botCommand);
        }
    }
}