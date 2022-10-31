using Common.Enum;
using Common.Interfaces;

namespace Common;

[Serializable]
public class Logger : ILogger
{
    private string logName { get; set; }
    private string version { get; set; } = EnvironmentVariables.GetVariable("version");
    private string location { get; set; }
    private IFileHandler _fileHandler;

    public Logger(IFileHandler FileHandler, string Name = "")
    {
        _fileHandler = FileHandler;

        logName = Name;
        if (Name == "")
        {
            logName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }

        location = $"bin/pirate{version}/logs";
    }

    public void Log(string message, string orginFile, LogType logType)
    {
        var time = DateTime.Now.ToString();
        var text = ($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {message}\n");

        _fileHandler.AppendToFile(logName, ".log", text, location);
    }
}
