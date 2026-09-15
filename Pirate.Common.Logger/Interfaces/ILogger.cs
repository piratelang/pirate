using Pirate.Common.Logger.Enum;

namespace Pirate.Common.Logger.Interfaces;

public interface ILogger
{
    ILoggerConfiguration LoggerConfiguration { get; set; }

    bool Fatal(System.Exception exception);
    bool Error(System.Exception exception);
    bool Warning(string  message);
    bool Info(string message);
    bool Debug(string message);
}