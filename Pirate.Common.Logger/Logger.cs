using Pirate.Common.FileHandler.Model;
using Pirate.Common.FileHandler;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger.Interfaces;
using Pirate.Common.Logger.Exception;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pirate.Common.Logger;

public class Logger : ILogger, IDisposable
{
    /// <summary>
    /// The configuration options for the logger.
    /// </summary>
    public ILoggerConfiguration LoggerConfiguration { get; set; }

    private string LogFileName { get; set; }
    private readonly StringBuilder _cacheText = new();
    private readonly StringBuilder _fileBuffer = new();
    private const int MaxCacheSize = 32_768;
    private const int FileFlushThreshold = 4_096;
    private bool _isDisposed;

    private readonly IFileWriteHandler _fileWriteHandler;


    public Logger() : this(new FileWriteHandler()) { }
    public Logger(IFileWriteHandler fileWriteHandler) : this(fileWriteHandler, new LoggerConfiguration()) { }
    public Logger(ILoggerConfiguration loggerConfiguration) : this(new FileWriteHandler(), loggerConfiguration) { }

    public Logger(IFileWriteHandler fileWriteHandler, ILoggerConfiguration loggerConfiguration)
    {
        _fileWriteHandler = fileWriteHandler;
        LoggerConfiguration = loggerConfiguration;

        LogFileName = $"{DateTime.UtcNow:yyyy.M.d.H.m.s}";
        AppDomain.CurrentDomain.ProcessExit += (_, _) => FlushBufferedFileLogs();
    }

    /// <summary>
    /// Logs a message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <param name="logType">INFO, WARNING or ERROR</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    [Obsolete]
    public bool Log(string message, LogType logType, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        return PirateLog(message, logType, callerFilePath, callerMemberName);
    }

    private bool PirateLog(string message, LogType logType, string callerFilePath, string callerMemberName)
    {
        if (string.IsNullOrEmpty(message)) throw new LoggerException("Message cannot be null or empty");

        var time = DateTime.UtcNow.ToString("O");
        var formattedMessage = MessageFormatter.FormatMessage(message);

        if (string.IsNullOrEmpty(formattedMessage)) throw new LoggerException("Message cannot be null or empty");

        var source = MessageFormatter.GetCallingLocation(callerFilePath, callerMemberName);
        var text = $"{time}: {logType}: {source}: {formattedMessage}";

        return WriteToTarget(text);
    }

    /// <summary>
    /// Logs an exception as a fatal error
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <returns>True if the exception was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the exception is null or empty</exception>
    public bool Fatal(System.Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        var result = exception.Message.Split(Environment.NewLine).All(line => PirateLog(line, LogType.FATAL, callerFilePath, callerMemberName));
        if (exception.InnerException != null) result = PirateLog(exception.InnerException.Message, LogType.INNEREXCEPTION, callerFilePath, callerMemberName);

        return result;
    }

    /// <summary>
    /// Logs an exception
    /// </summary>
    /// <param name="exception">The exception to log</param>
    /// <returns>True if the exception was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the exception is null or empty</exception>
    public bool Error(System.Exception exception, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        var result = exception.Message.Split(Environment.NewLine).All(line => PirateLog(line, LogType.ERROR, callerFilePath, callerMemberName));
        if (exception.InnerException != null) result = PirateLog(exception.InnerException.Message, LogType.INNEREXCEPTION, callerFilePath, callerMemberName);
        if (exception.StackTrace != null) result = exception.StackTrace.Split(Environment.NewLine).All(line => PirateLog(line, LogType.STACKTRACE, callerFilePath, callerMemberName));

        return result;
    }

    /// <summary>
    /// Logs a warning
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Warning(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        return PirateLog(message, LogType.WARNING, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs an info message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Info(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        return PirateLog(message, LogType.INFO, callerFilePath, callerMemberName);
    }

    /// <summary>
    /// Logs a debug message
    /// </summary>
    /// <param name="message">The message to log</param>
    /// <returns>True if the message was logged successfully</returns>
    /// <exception cref="LoggerException">Thrown when the message is null or empty</exception>
    public bool Debug(string message, [CallerFilePath] string callerFilePath = "", [CallerMemberName] string callerMemberName = "")
    {
        return PirateLog(message, LogType.DEBUG, callerFilePath, callerMemberName);
    }

    private bool WriteToTarget(string text)
    {
        text += Environment.NewLine;
        _cacheText.Append(text);
        if (_cacheText.Length > MaxCacheSize)
        {
            _cacheText.Remove(0, _cacheText.Length - MaxCacheSize);
        }

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
                _fileBuffer.Append(text);
                if (_fileBuffer.Length >= FileFlushThreshold) FlushBufferedFileLogs();
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
                        _cacheText.ToString()));
                    _cacheText.Clear();
                }
                break;

            default:
                throw new LoggerException("UseFileEnum is not set");
        }

        return true;
    }

    private void FlushBufferedFileLogs()
    {
        if (_fileBuffer.Length == 0 || LoggerConfiguration.UseFile != UseFileEnum.True) return;

        _fileWriteHandler.AppendToFile(new FileWriteModel(
            LogFileName,
            FileExtension.LOG,
            LoggerConfiguration.FolderName,
            _fileBuffer.ToString()));
        _fileBuffer.Clear();
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        FlushBufferedFileLogs();
        _isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
