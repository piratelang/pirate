using System;
using Common.Enum;

namespace Common;

[Serializable]
public class Logger
{
    public string logName { get; set; }
    public static string version { get; set; } = "0.0.0";

    public Logger(string Version, string Name = null)
    {
        version = Version;
        var dateTime = DateTime.Now;
        logName = Name;
        if(Name == null)
        {
            logName = $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}.{dateTime.Hour}.{dateTime.Minute}.{dateTime.Second}";
        }
    }

    public void Log(string message, string orginFile, LogType logType)
    {
        var location = $"./bin/pirate{version}/logs/{logName}.log";
        bool exists = System.IO.Directory.Exists($"./bin/pirate{version}/logs");
        if (!exists)
            System.IO.Directory.CreateDirectory($"./bin/pirate{version}/logs");

        exists = File.Exists(location);
        if (!exists)
        {
            var createdFile = File.Create(location);
            createdFile.Close();
        }
        var file = File.AppendText(location);

        var time = DateTime.Now.ToString();
        file.Write($"{time.Replace(" uur", "")}: {logType.ToString()}: {orginFile}.cs: {message}\n");
        file.Close();
    }
}
