using Common.Enum;

namespace Common.Interfaces;

/// <inheritdoc cref="Logger"/>
public interface ILogger
{
    bool Log(string message, LogType logType);
}
