using FakeItEasy;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.FileHandler.Model;
using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger.Interfaces;
using Xunit;

namespace Pirate.Common.Test;

public class LoggerTest
{
    [Fact]
    public void ShouldUseCallerInformationForInterfaceCalls()
    {
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        ILogger logger = new Logger.Logger(fileWriteHandler, new Logger.LoggerConfiguration
        {
            UseConsole = UseConsoleEnum.False,
            UseFile = UseFileEnum.OnFatal
        });

        var result = logger.Info("Test message");
        logger.Fatal(new System.Exception("force flush"));

        Assert.True(result);
        A.CallTo(() => fileWriteHandler.AppendToFile(A<FileWriteModel>.That.Matches(model =>
            model.Text.Contains("LoggerTest.ShouldUseCallerInformationForInterfaceCalls"))))
            .MustHaveHappened();
    }

    [Fact]
    public void ShouldBufferFileWritesWhenUseFileIsTrue()
    {
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var logger = new Logger.Logger(fileWriteHandler, new Logger.LoggerConfiguration
        {
            UseConsole = UseConsoleEnum.False,
            UseFile = UseFileEnum.True
        });

        logger.Info("one");
        logger.Info("two");

        A.CallTo(() => fileWriteHandler.AppendToFile(A<FileWriteModel>.Ignored)).MustNotHaveHappened();

        logger.Dispose();

        A.CallTo(() => fileWriteHandler.AppendToFile(A<FileWriteModel>.That.Matches(model =>
            model.Text.Contains("one") && model.Text.Contains("two"))))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LoggerConfigurationDefaultsToConsoleAndOnFatalFileLogging()
    {
        var configuration = new Logger.LoggerConfiguration();

        Assert.Equal(UseConsoleEnum.True, configuration.UseConsole);
        Assert.Equal(UseFileEnum.OnFatal, configuration.UseFile);
    }
}