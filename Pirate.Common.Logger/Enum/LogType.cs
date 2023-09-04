namespace Pirate.Common.Logger.Enum;

/// <summary>
/// The type of log, which can be ERROR, WARNING or INFO.
/// </summary>
public enum LogType
{
    FATAL,
    ERROR,
    INNEREXCEPTION,
    STACKTRACE,
    WARNING,
    INFO,
    DEBUG
}
