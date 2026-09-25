using Pirate.Shared.Logging;
using Xunit;

namespace Pirate.Shared.Logging.Test;

public class NullLoggerTests
{
    [Fact]
    public void NullLogger_IsNeverEnabled()
    {
        Assert.False(NullLogger.Instance.IsEnabled(LogLevel.Debug));
        Assert.False(NullLogger.Instance.IsEnabled(LogLevel.Fatal));
    }

    [Fact]
    public void NullLogger_LogDoesNothing()
    {
        // Should not throw
        NullLogger.Instance.Log(LogLevel.Fatal, "this goes nowhere");
        NullLogger.Instance.Log(LogLevel.Fatal, new InvalidOperationException(), "also nowhere");
    }
}

public class ConsoleLoggerTests
{
    [Fact]
    public void ConsoleLogger_RespectsMinLevel()
    {
        var logger = new ConsoleLogger(LogLevel.Warning);
        Assert.False(logger.IsEnabled(LogLevel.Debug));
        Assert.True(logger.IsEnabled(LogLevel.Error));
    }

    [Fact]
    public void ConsoleLogger_LogsAtAndAboveMinLevel()
    {
        var logger = new ConsoleLogger(LogLevel.Info);
        Assert.True(logger.IsEnabled(LogLevel.Info));
        Assert.True(logger.IsEnabled(LogLevel.Warning));
        Assert.True(logger.IsEnabled(LogLevel.Fatal));
    }

    [Fact]
    public void ConsoleLogger_LogDoesNotThrow()
    {
        var logger = new ConsoleLogger(LogLevel.Trace);
        logger.Log(LogLevel.Info, "test message");
        logger.Log(LogLevel.Error, new InvalidOperationException("boom"), "test with exception");
    }
}
