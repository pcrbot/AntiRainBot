using System;
using System.Text;
using System.Threading.Tasks;
using AntiRain.IO.Config;
using AntiRain.IO.Config.ConfigModule;
using AntiRain.Resource;
using AntiRain.TimerEvent;
using Sora.Server;
using Sora.Tool;

namespace AntiRain.ServerInterface
{
    static class SoraServerInterface
    {
        static async Task Main()
        {
            //修改控制台标题
            Console.Title = @"AntiRain";
            ConsoleLog.Info("AntiRain初始化","AntiRain初始化...");
            //初始化配置文件
            ConsoleLog.Info("AntiRain初始化","初始化服务器全局配置...");
            //全局文件初始化不需要uid，填0仅占位，不使用构造函数重载
            Config config = new Config(0);
            config.GlobalConfigFileInit();
            config.LoadGlobalConfig(out GlobalConfig globalConfig, false);


            ConsoleLog.SetLogLevel(globalConfig.LogLevel);
            //显示Log等级
            ConsoleLog.Debug("Log Level", globalConfig.LogLevel);

            //初始化字符编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //指令匹配初始化
            Command.KeywordResourseInit();
            Command.RegexResourseInit();
            Command.PCRGuildBattlecmdResourseInit();

            ConsoleLog.Info("AntiRain初始化","启动反向WS服务器...");
            //初始化服务器
            SoraWSServer server = new SoraWSServer(new ServerConfig
            {
                Location         = globalConfig.Location,
                Port             = globalConfig.Port,
                AccessToken      = globalConfig.AccessToken,
                UniversalPath    = globalConfig.UniversalPath,
                ApiPath          = globalConfig.ApiPath,
                EventPath        = globalConfig.EventPath,
                HeartBeatTimeOut = globalConfig.HeartBeatTimeOut,
                ApiTimeOut       = globalConfig.ApiTimeOut
            });

            //服务器回调
            //初始化
            server.Event.OnClientConnect += InitalizationEvent.Initalization;
            //群聊事件
            server.Event.OnGroupMessage += GroupMessageEvent.GroupMessageParse;
            //私聊事件
            server.Event.OnPrivateMessage += PrivateMessageEvent.PrivateMessageParse;
            //关闭连接事件处理
            server.OnCloseConnectionAsync += TimerEventParse.StopTimer;

            await server.StartServerAsync();
        }
    }
}
