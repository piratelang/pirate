using Pirate.Common.Logger.Enum;

namespace Pirate.Common.Logger.Interfaces;

public interface ILogger
{
    ILoggerConfiguration LoggerConfiguration { get; set; }

    bool Log(string message, LogType logType);
    bool Error(System.Exception exception);
}