using System.Diagnostics;
using Pirate.Common.FileHandlers;
using Pirate.Common.Enum;
using Pirate.Common.FileHandlers.Interfaces;
using Pirate.Common.Interfaces;

namespace Pirate.Common;

/// <summary>
/// Writes a log message to the log file.
/// </summary>
public class Logger : ILogger
{
    private string logFileName { get; set; }
    private string version { get; set; }
    private string location { get; set; }
    private readonly IFileWriteHandler _fileWriteHandler;

    public Logger(IFileWriteHandler FileWriteHandler, IEnvironmentVariables environmentVariables, string Name = "")
    {
        _fileWriteHandler = FileWriteHandler;

        logFileName = Name;
        if (Name == "")
        {
            logFileName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }

        version = environmentVariables.GetVariable("version") ?? "0.0.0";
        location = $"bin/pirate{version}/logs";
    }

    public bool Log(string message, LogType logType)
    {
        var time = DateTime.Now.ToString();
        var formattedMessage = FormatMessage(message);
        var text = $"{time.Replace(" uur", "")}: {logType.ToString()}: {GetCallingClassName()}.cs: {formattedMessage}\n";

        _fileWriteHandler.AppendToFile(new FileWriteModel(logFileName, FileExtension.LOG, location, text));

        return true;
    }

    public string FormatMessage(string message)
    {
        if (message.Contains('\n')) message = message.Replace('\n', ' ');
        if (message.Contains('\r')) message = message.Replace('\r', ' ');

        return message;
    }

    public string GetCallingClassName()
    {
        var stackTrace = new StackTrace();
        var callingClass = stackTrace.GetFrame(2).GetMethod().DeclaringType.Name;
        if (callingClass == null) callingClass = "Unknown";

        return callingClass;
    }
}
