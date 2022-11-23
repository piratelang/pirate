using Common.Enum;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;
using FakeItEasy;
using Xunit;

namespace Common.Test;

public class LoggerTest
{
    [Fact]
    public void ShouldLog()
    {
        // Arrange
        var FileWriteHandler = A.Fake<IFileWriteHandler>();
        var EnvironmentVariables = A.Fake<IEnvironmentVariables>();
        var logger = new Logger(FileWriteHandler, EnvironmentVariables);

        A.CallTo(() => FileWriteHandler.WriteToFile(A<FileWriteModel>.Ignored, A<bool>.Ignored)).Returns(true);
        A.CallTo(() => EnvironmentVariables.GetVariable("version")).Returns("1.0.0");
        var message = "Test message";
        var type = LogType.INFO;

        // Act
        var result = logger.Log(message, type);

        // Assert
        Assert.True(result);
    }
        
}