using Common.Enum;

namespace Common;

public interface ILogger
{
    string logName { get; set; }

    void Log(string message, string orginFile, LogType logType);
}
