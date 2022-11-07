using Common.Enum;
using Common.FileHandlers;
using Xunit;

namespace Common.Test;

public class LoggerTest
{
    [Fact]
    public void ShouldLogToFIle()
    {
        //Arrange
        var logger = new Logger(new FileWriteHandler(), "Test");
        //Act
        logger.Log("Test", "Test", LogType.INFO);
        var exists = System.IO.File.Exists($"./bin/pirateTest/logs/Test.log");

        //Assert
        Assert.True(exists);
    }
}