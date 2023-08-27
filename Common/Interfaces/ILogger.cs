using Pirate.Common.Enum;

namespace Pirate.Common.Interfaces;

/// <inheritdoc cref="Logger"/>
public interface ILogger
{
    bool Log(string message, LogType logType);
}
