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

        version = environmentVariables.GetVariable("version");
        location = $"bin/pirate{version}/logs";
    }

    public bool Log(string message, string orginFile, LogType logType)
    {
        var time = DateTime.Now.ToString();
        var checkedmessage = CheckMessage(message);
        var text = ($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {checkedmessage}\n");

        _fileWriteHandler.AppendToFile(new FileWriteModel(logFileName, FileExtension.LOG, location, text));

        return true;
    }

    public string CheckMessage(string message)
    {
        if (message.Contains('\n'))
        {
            message = message.Replace('\n', ' ');
        }
        if (message.Contains('\r'))
        {
            message = message.Replace('\r', ' ');
        }
        
        return message;
    }
}
