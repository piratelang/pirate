using FakeItEasy;
using Pirate.Common.Enum;
using Pirate.Common.FileHandlers;
using Pirate.Common.FileHandlers.Interfaces;
using Pirate.Common.Interfaces;
using Xunit;

namespace Pirate.Common.Test;

public class LoggerTest
{
    [Fact]
    public void ShouldLog()
    {
        // Arrange
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var environmentVariables = A.Fake<IEnvironmentVariables>();
        var logger = new Logger(fileWriteHandler, environmentVariables);

        A.CallTo(() => fileWriteHandler.WriteToFile(A<FileWriteModel>.Ignored, A<bool>.Ignored)).Returns(true);
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var message = "Test message";
        var type = LogType.INFO;

        // Act
        var result = logger.Log(message, type);

        // Assert
        Assert.True(result);
    }

}