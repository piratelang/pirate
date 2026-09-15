using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FakeItEasy;
using Newtonsoft.Json;
using Pirate.Common.Exception.Exceptions;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.FileHandler.Model;
using Pirate.Common.Interfaces;
using Pirate.Common.Logger.Interfaces;
using Xunit;

namespace Pirate.Common.Test;

public class ObjectSerializerTest
{
    [Fact]
    public void Deserialize_ShouldThrowModuleNotBuiltException_WhenCacheFileDoesNotExist()
    {
        var fileReadHandler = A.Fake<IFileReadHandler>();
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var logger = A.Fake<ILogger>();
        var environmentVariables = CreateEnvironmentVariables();
        var serializer = new ObjectSerializer(logger, environmentVariables, fileWriteHandler, fileReadHandler);

        A.CallTo(() => fileReadHandler.FileExists("main.pirate", FileExtension.JSON, serializer.Location)).Returns(false);

        Assert.Throws<ModuleNotBuiltException>(() => serializer.Deserialize<object>("main.pirate"));
        A.CallTo(() => fileWriteHandler.WriteToFile(A<FileWriteModel>.Ignored)).MustNotHaveHappened();
    }

    [Fact]
    public void Deserialize_ShouldWrapJsonException_AsSerializationExceptionWithInnerException()
    {
        var fileReadHandler = A.Fake<IFileReadHandler>();
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var logger = A.Fake<ILogger>();
        var environmentVariables = CreateEnvironmentVariables();
        var serializer = new ObjectSerializer(logger, environmentVariables, fileWriteHandler, fileReadHandler);

        A.CallTo(() => fileReadHandler.FileExists("main.pirate", FileExtension.JSON, serializer.Location)).Returns(true);
        A.CallTo(() => fileReadHandler.ReadAllTextFromFile("main.pirate", FileExtension.JSON, serializer.Location)).Returns(Task.FromResult("{not-valid-json"));

        var exception = Assert.Throws<SerializationException>(() => serializer.Deserialize<object>("main.pirate"));

        Assert.IsAssignableFrom<JsonException>(exception.InnerException);
    }

    [Fact]
    public void Deserialize_ShouldThrowSerializationException_WhenNodesIsNull()
    {
        var fileReadHandler = A.Fake<IFileReadHandler>();
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var logger = A.Fake<ILogger>();
        var environmentVariables = CreateEnvironmentVariables();
        var serializer = new ObjectSerializer(logger, environmentVariables, fileWriteHandler, fileReadHandler);

        A.CallTo(() => fileReadHandler.FileExists("main.pirate", FileExtension.JSON, serializer.Location)).Returns(true);
        A.CallTo(() => fileReadHandler.ReadAllTextFromFile("main.pirate", FileExtension.JSON, serializer.Location)).Returns(Task.FromResult("{\"Nodes\":null}"));

        Assert.Throws<SerializationException>(() => serializer.Deserialize<ScopeLike>("main.pirate"));
    }

    private static IEnvironmentVariables CreateEnvironmentVariables()
    {
        var environmentVariables = A.Fake<IEnvironmentVariables>();
        A.CallTo(() => environmentVariables.GetVariable("location")).Returns("/tmp");
        return environmentVariables;
    }

    private class ScopeLike
    {
        public List<string>? Nodes { get; set; }
    }
}
