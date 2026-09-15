using System.IO;

namespace Pirate.Common.Logger;

internal static class MessageFormatter
{
    internal static string FormatMessage(string message)
    {
        if (message.Contains('\n')) message = message.Replace('\n', ' ');
        if (message.Contains('\r')) message = message.Replace('\r', ' ');

        return message;
    }

    internal static string GetCallingLocation(string callerFilePath, string callerMemberName)
    {
        var callerFileName = Path.GetFileNameWithoutExtension(callerFilePath);
        if (string.IsNullOrEmpty(callerFileName) && string.IsNullOrEmpty(callerMemberName)) return "Unknown";
        if (string.IsNullOrEmpty(callerFileName)) return callerMemberName;
        if (string.IsNullOrEmpty(callerMemberName)) return callerFileName;

        return $"{callerFileName}.{callerMemberName}";
    }
}