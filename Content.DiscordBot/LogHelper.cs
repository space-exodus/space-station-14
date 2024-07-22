// using Discord;
// using Robust.Shared.Log;
// using Robust.Shared.Utility;

// namespace Content.DiscordBot;

// public static class LogHelper
// {
//     public static ISawmill GetLogger(string name = "discord")
//     {
//         return DiscordIoC.Resolve<ILogManager>().GetSawmill(name);
//     }

//     public static Task LogAsync(Discord.LogMessage message)
//     {
//         return LogAsync(message, null);
//     }
//     public static Task LogAsync(Discord.LogMessage message, ISawmill? logger = null)
//     {
//         var usingLogger = logger ?? GetLogger();

//         var logLevel = message.Severity switch
//         {
//             LogSeverity.Critical => LogLevel.Fatal,
//             LogSeverity.Error => LogLevel.Error,
//             LogSeverity.Warning => LogLevel.Warning,
//             LogSeverity.Info => LogLevel.Info,
//             LogSeverity.Verbose => LogLevel.Verbose,
//             LogSeverity.Debug => LogLevel.Debug,
//             _ => throw new NotImplementedException(),
//         };

//         if (message.Exception != null)
//         {
//             usingLogger.Error(message.Exception.ToStringBetter());
//         }
//         else
//             usingLogger.Log(logLevel, message.Message);

//         return Task.CompletedTask;
//     }
// }
