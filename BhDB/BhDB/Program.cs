using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
    class Program
    {
        private DiscordSocketClient dsClient = new DiscordSocketClient();
        private CommandService cs = new CommandService();

        static void Main(string[] args)
        {
            new Program().BotMain().GetAwaiter().GetResult();
        }

        public async Task BotMain()
        {
            DiscordSocketConfig dsConfig = new DiscordSocketConfig() { LogLevel = Discord.LogSeverity.Verbose };
            CommandServiceConfig csConfig = new CommandServiceConfig() { LogLevel = Discord.LogSeverity.Verbose };
            dsClient = new DiscordSocketClient(dsConfig);
            cs = new CommandService(csConfig);

            dsClient.Log += OnClientLogReceived;
            cs.Log += OnClientLogReceived;

            await dsClient.LoginAsync(TokenType.Bot, "Bot token");
            await dsClient.StartAsync();

            dsClient.MessageReceived += OnClientMessage;

            await Task.Delay(-1);
        }

        private async Task OnClientMessage(SocketMessage socket)
        {
            SocketUserMessage? message = socket as SocketUserMessage;
            if (message == null) { return; }

            int pos = 0;

            if ((!message.HasCharPrefix('!', ref pos) && !message.HasMentionPrefix(dsClient.CurrentUser, ref pos)) ||
                message.Author.IsBot)
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(dsClient, message);

            await context.Channel.SendMessageAsync("명령어 수신됨: " + message.Content);
        }
        /// <summary>
        /// 봇의 로그를 출력하는 삼수
        /// </summary>
        /// <param name="log">봇의 클라이언트에서 수신괸 로그</param>
        /// <returns>Task 완료 시그널 전달</returns>
        private Task OnClientLogReceived(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
    }
}