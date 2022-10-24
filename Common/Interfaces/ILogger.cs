using Common.Enum;

namespace Common.Interfaces;

public interface ILogger
{

    void Log(string message, string orginFile, LogType logType);
}
