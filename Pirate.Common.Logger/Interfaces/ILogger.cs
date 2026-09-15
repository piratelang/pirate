namespace Pirate.Common.Logger.Interfaces;

public interface ILogger
{
    ILoggerConfiguration LoggerConfiguration { get; set; }

    bool Fatal(string message);
    bool Fatal(System.Exception exception);
    bool Error(string message);
    bool Error(System.Exception exception);
    bool Warning(string  message);
    bool Info(string message);
    bool Debug(string message);
}