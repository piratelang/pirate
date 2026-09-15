using Pirate.Common.Logger.Enum;
using System.Runtime.CompilerServices;

namespace Pirate.Common.Logger.Interfaces;

public interface ILogger
{
    ILoggerConfiguration LoggerConfiguration { get; set; }

    [Obsolete]
    bool Log(string message, LogType logType, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");

    bool Fatal(System.Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
    bool Error(System.Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
    bool Warning(string  message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
    bool Info(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
    bool Debug(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "");
}