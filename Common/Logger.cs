using Common.Enum;
using Common.Interfaces;

namespace Common;

[Serializable]
public class Logger : ILogger
{
    private string logName { get; set; }
    private string version { get; set; } = EnvironmentVariables.GetVariable("version");
    private string location { get; set; }

    public Logger(string Name = "placeholder")
    {
        logName = Name;
        if (Name == "placeholder")
        {
            logName = $"{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}.{DateTime.Now.Hour}.{DateTime.Now.Minute}.{DateTime.Now.Second}";
        }
        location = $"./bin/pirate{version}/logs/{logName}.log";

        bool exists = Directory.Exists($"./bin/pirate{version}/logs");
        if (!exists)
            Directory.CreateDirectory($"./bin/pirate{version}/logs");

        var createdFile = File.Create(location);
        createdFile.Close();
    }

    public void Log(string message, string orginFile, LogType logType)
    {
        var file = File.AppendText(location);
        var time = DateTime.Now.ToString();
        file.Write($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {message}\n");
        file.Close();
    }
}
