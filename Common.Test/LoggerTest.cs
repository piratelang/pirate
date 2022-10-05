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
        var logger = new Logger("0.1.2");
        
        //Act
        Logger.Log("Test", "Test", LogType.INFO);
        var exists = System.IO.File.Exists($"./bin/pirate0.1.2/logs/{Logger.logName}.log");

        //Assert
        Assert.True(exists);
    }
}