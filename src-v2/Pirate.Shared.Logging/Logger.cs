namespace Pirate.Shared.Logging;

/// <summary>
/// Structured log level. Matches the common set: trace through fatal, with
/// "info" as the default for user-facing CLI output.
/// </summary>
public enum LogLevel
{
    Trace,
    Debug,
    Info,
    Warning,
    Error,
    Fatal,
}

/// <summary>
/// Minimal logging interface — intentionally dependency-free so any
/// implementation can back it (console, file, Spectre.Console markup, or a
/// no-op for CI). Not Microsoft.Extensions.Logging: that's a package
/// dependency this project deliberately avoids (see Pirate.Shared.File's
/// design philosophy — BCL only, no package references).
/// </summary>
public interface ILogger
{
    void Log(LogLevel level, string message);
    void Log(LogLevel level, Exception? exception, string message);
    bool IsEnabled(LogLevel level) => true;
}

/// <summary>
/// A logger that discards everything. Useful for CI runs, tests, or any
/// context where output should be suppressed.
/// </summary>
public sealed class NullLogger : ILogger
{
    public static readonly NullLogger Instance = new();
    public void Log(LogLevel level, string message) { }
    public void Log(LogLevel level, Exception? exception, string message) { }
    public bool IsEnabled(LogLevel level) => false;
}

/// <summary>
/// Writes to <see cref="Console.Error"/> with a level prefix. Not for
/// user-facing output (use <c>Spectre.Console.AnsiConsole</c> directly for
/// that); this is for debug/diagnostic traces that should go to stderr.
/// </summary>
public sealed class ConsoleLogger : ILogger
{
    private readonly LogLevel _minLevel;

    public ConsoleLogger(LogLevel minLevel = LogLevel.Debug) => _minLevel = minLevel;

    public void Log(LogLevel level, string message)
    {
        if (level < _minLevel) return;
        Console.Error.WriteLine($"[{level}] {message}");
    }

    public void Log(LogLevel level, Exception? exception, string message)
    {
        if (level < _minLevel) return;
        Console.Error.WriteLine($"[{level}] {message}");
        if (exception is not null)
        {
            Console.Error.WriteLine($"  {exception.GetType().Name}: {exception.Message}");
        }
    }

    public bool IsEnabled(LogLevel level) => level >= _minLevel;
}
