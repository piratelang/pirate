using Common.Enum;

namespace Common;

public interface ILogger
{

    void Log(string message, string orginFile, LogType logType);
}
