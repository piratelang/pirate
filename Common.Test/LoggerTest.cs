using System.IO;
using Common.Enum;
using Xunit;
using System;

namespace Common.Test;

public class LoggerTest
{
    [Fact]
    public void ShouldLogToFIle()
    {
        //Arrange
        var logger = new Logger("Test");
        //Act
        logger.Log("Test", "Test", LogType.INFO);
        var exists = System.IO.File.Exists($"./bin/pirateTest/logs/Test.log");

        //Assert
        Assert.True(exists);
    }
}