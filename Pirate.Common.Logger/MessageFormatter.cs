using System.Diagnostics;

namespace Pirate.Common.Logger;

internal static class MessageFormatter
{
    internal static string FormatMessage(string message)
    {
        if (message.Contains('\n')) message = message.Replace('\n', ' ');
        if (message.Contains('\r')) message = message.Replace('\r', ' ');

        return message;
    }

    internal static string GetCallingClassName()
    {
        var stackTrace = new StackTrace();
        var callingClass = stackTrace.GetFrame(3)?.GetMethod()?.DeclaringType?.Name ?? "Unknown";

        return callingClass;
    }
}