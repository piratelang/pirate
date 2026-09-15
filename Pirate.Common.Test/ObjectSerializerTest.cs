using System;
using System.IO;
using FakeItEasy;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Interfaces;
using Pirate.Common.Logger.Interfaces;
using Xunit;

namespace Pirate.Common.Test;

public class ObjectSerializerTest
{
    [Fact]
    public void Deserialize_WhenReadFails_ThrowsFileNotFoundException()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var environmentVariables = A.Fake<IEnvironmentVariables>();
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var fileReadHandler = A.Fake<IFileReadHandler>();

        var location = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        A.CallTo(() => environmentVariables.GetVariable("location")).Returns(location);
        A.CallTo(() => fileReadHandler.FileExists("missing", FileExtension.JSON, location + "/cache")).Returns(true);
        A.CallTo(() => fileReadHandler.ReadAllTextFromFile("missing", FileExtension.JSON, location + "/cache"))
            .Throws(new FileNotFoundException("File not found"));

        var serializer = new ObjectSerializer(logger, environmentVariables, fileWriteHandler, fileReadHandler);

        // Act + Assert
        Assert.Throws<FileNotFoundException>(() => serializer.Deserialize<object>("missing"));
    }
}
