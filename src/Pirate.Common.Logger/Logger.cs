using Pirate.Common.FileHandler.Model;
using Pirate.Common.FileHandler;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger.Interfaces;
using Pirate.Common.Logger.Exception;
using System.Diagnostics;

namespace Pirate.Common.Logger;

public class Logger : ILogger
{
    /// <summary>
    /// The configuration options for the logger.
    /// </summary>
    public ILoggerConfiguration LoggerConfiguration { get; set; }

    private string LogFileName { get; set; }
    private string CacheText { get; set; }

    private readonly IFileWriteHandler _fileWriteHandler;


    public Logger() : this(new FileWriteHandler()) { }
    public Logger(IFileWriteHandler fileWriteHandler) : this(fileWriteHandler, new LoggerConfiguration()) { }
    public Logger(ILoggerConfiguration loggerConfiguration) : this(new FileWriteHandler(), loggerConfiguration) { }

    public Logger(IFileWriteHandler fileWriteHandler, ILoggerConfiguration loggerConfiguration)
    {
        _fileWriteHandler = fileWriteHandler;
        LoggerConfiguration = loggerConfiguration;

        LogFileName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
    }

    /// <summary>
    /// Logs a message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="logType">INFO, WARNING or ERROR</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    [Obsolete]
    public bool Log(string message, LogType logType)
    {
        if (string.IsNullOrEmpty(message)) throw new LoggerException("Message cannot be null or empty");

        var time = DateTime.Now.ToString();
        var formattedMessage = MessageFormatter.FormatMessage(message);

        if (string.IsNullOrEmpty(formattedMessage)) throw new LoggerException("Message cannot be null or empty");

        var text = $"{time.Replace(" uur", "")}: {logType}: {MessageFormatter.GetCallingClassName()}.cs: {formattedMessage}";

        return WriteToTarget(text);
    }

    private bool PirateLog(string message, LogType logType)
    {
        if (string.IsNullOrEmpty(message)) throw new LoggerException("Message cannot be null or empty");

        var time = DateTime.Now.ToString();
        var formattedMessage = MessageFormatter.FormatMessage(message);

        if (string.IsNullOrEmpty(formattedMessage)) throw new LoggerException("Message cannot be null or empty");

        var text = $"{time.Replace(" uur", "")}: {logType}: {MessageFormatter.GetCallingClassName()}.cs: {formattedMessage}";

        return WriteToTarget(text);
    }

    /// <summary>
    /// Logs an exception as a fatal error
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <returns>True if the exception was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the exception is null or empty</exception>
    public bool Fatal(System.Exception exception)
    {
        var result = exception.Message.Split(Environment.NewLine).All(line => PirateLog(line, LogType.FATAL));
        if (exception.InnerException != null) result = PirateLog(exception.InnerException.Message, LogType.INNEREXCEPTION);

        return result;
    }

    /// <summary>
    /// Logs an exception
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <returns>True if the exception was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the exception is null or empty</exception>
    public bool Error(System.Exception exception)
    {
        var result = exception.Message.Split(Environment.NewLine).All(line => PirateLog(line, LogType.ERROR));
        if (exception.InnerException != null) result = PirateLog(exception.InnerException.Message, LogType.INNEREXCEPTION);
        if (exception.StackTrace != null) result = exception.StackTrace.Split(Environment.NewLine).All(line => PirateLog(line, LogType.STACKTRACE));

        return result;
    }

    /// <summary>
    /// Logs a warning
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Warning(string message)
    {
        return PirateLog(message, LogType.WARNING);
    }

    /// <summary>
    /// Logs an info message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Info(string message)
    {
        return PirateLog(message, LogType.INFO);
    }

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Debug(string message)
    {
        return PirateLog(message, LogType.DEBUG);
    }

    private bool WriteToTarget(string text)
    {
        text += Environment.NewLine;
        CacheText += text;

        switch (LoggerConfiguration.UseConsole)
        {
            case UseConsoleEnum.True:
                Console.WriteLine(text);
                break;
            case UseConsoleEnum.False:
                break;

            default:
                throw new LoggerException("UseConsoleEnum is not set");
        }

        switch (LoggerConfiguration.UseFile)
        {
            case UseFileEnum.True:
                _fileWriteHandler.AppendToFile(new FileWriteModel(
                    LogFileName,
                    FileExtension.LOG,
                    LoggerConfiguration.FolderName,
                    text));
                break;
            case UseFileEnum.False:
                break;
            case UseFileEnum.OnFatal:
                if (text.Contains("FATAL"))
                {
                    _fileWriteHandler.AppendToFile(new FileWriteModel(
                        LogFileName,
                        FileExtension.LOG,
                        LoggerConfiguration.FolderName,
                        CacheText));
                    CacheText = string.Empty;
                }
                break;

            default:
                throw new LoggerException("UseFileEnum is not set");
        }

        return true;
    }
}
