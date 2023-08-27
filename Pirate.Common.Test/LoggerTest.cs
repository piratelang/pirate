using FakeItEasy;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.FileHandler.Model;
using Pirate.Common.Interfaces;
using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger;
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
        var logger = new Logger.Logger();

        A.CallTo(() => fileWriteHandler.WriteToFile(A<FileWriteModel>.Ignored)).Returns(true);
        A.CallTo(() => environmentVariables.GetVariable("version")).Returns("1.0.0");
        var message = "Test message";
        var type = LogType.INFO;

        // Act
        var result = logger.Log(message, type);

        // Assert
        Assert.True(result);
    }

}