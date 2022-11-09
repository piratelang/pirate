using Common.Enum;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;

namespace Common;

[Serializable]
public class Logger : ILogger
{
    private string logFileName { get; set; }
    private string version { get; set; }
    private string location { get; set; }
    private readonly IFileWriteHandler _fileWriteHandler;

    public Logger(IFileWriteHandler FileWriteHandler, IEnvironmentVariables environmentVariables, string Name = "")
    {
        _fileWriteHandler = FileWriteHandler;
        
        logFileName = Name;
        if (Name == "")
        {
            logFileName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }

        location = $"bin/pirate{version}/logs";
        version = environmentVariables.GetVariable("version");
    }

    public bool Log(string message, string orginFile, LogType logType)
    {
        var time = DateTime.Now.ToString();
        var text = ($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {message}\n");

        _fileWriteHandler.AppendToFile(new FileWriteModel("log", ".log", location, text));

        return true;
    }
}
