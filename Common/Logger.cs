using System;
namespace Common;
public class Logger
{
    public static string logName { get; set; }
    public static string version { get; set; } = "0.0.0";

    public Logger(string Version)
    {
        version = Version;
        var dateTime = DateTime.Now;
        logName = $"{dateTime.Day}.{dateTime.Month}.{dateTime.Year}.{dateTime.Hour}.{dateTime.Minute}.{dateTime.Second}";
    }

    public static void Log(string message)
    {
        var location = $"./bin/pirate{version}/{logName}.log";
        var exists = File.Exists(location);
        if (!exists)
        {
            var createdFile = File.Create(location);
            createdFile.Close();
        }
        var file = File.AppendText(location);

        var time = DateTime.Now.ToString().Replace("uur", "");
        file.Write(time + ": " + message + '\n');
        file.Close();
    }
}
