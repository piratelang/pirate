using Common.Enum;

namespace Common.Interfaces;

public interface ILogger
{
    bool Log(string message, LogType logType);
}
