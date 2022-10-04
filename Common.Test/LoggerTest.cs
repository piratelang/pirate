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
        bool result;
        var logger = new Logger("0.1.2");
        
        //Act
        try
        {
            using (FileStream fs = new FileStream($"{Logger.logName}.log", FileMode.Append, FileAccess.Write))
            using (StreamWriter textWriter = new StreamWriter(fs))
            {
                Logger.Log("Test", "Test", LogType.INFO);
            }
            result = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error:" + ex.Message);
            result = false;
        }

        Assert.True(result);
    }
}