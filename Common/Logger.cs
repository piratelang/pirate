using Common.Enum;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;

namespace Common;

[Serializable]
public class Logger : ILogger
{
    private string logFileName { get; set; }
    private string version { get; set; } = EnvironmentVariables.GetVariable("version");
    private string location { get; set; }
    private readonly IFileWriteHandler _fileHandler;

    public Logger(IFileWriteHandler FileHandler, string Name = "")
    {
        _fileHandler = FileHandler;

        logFileName = Name;
        if (Name == "")
        {
            logFileName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }

        location = $"bin/pirate{version}/logs";
    }

    public void Log(string message, string orginFile, LogType logType)
    {
        var time = DateTime.Now.ToString();
        var text = ($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {message}\n");

        _fileHandler.AppendToFile(new FileWriteModel("log", ".log", location, text));
    }
}
