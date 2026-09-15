using FakeItEasy;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Interfaces;
using Pirate.Common.Logger.Interfaces;
using Xunit;

namespace Pirate.Common.Test;

public class ObjectSerializerTest
{
    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void ShouldThrowWhenDeserializingDisallowedType()
    {
        // Arrange
        const string json = "{\"$type\":\"System.Text.StringBuilder, System.Private.CoreLib\",\"m_MaxCapacity\":2147483647}";
        var serializer = CreateSerializer(json);

        // Act + Assert
        Assert.Throws<JsonSerializationException>(() => serializer.Deserialize<object>("main.pirate"));
    }

    [Fact]
    public void ShouldDeserializeObjectWithoutTypeMetadata()
    {
        // Arrange
        const string json = "{\"Name\":\"Blackbeard\"}";
        var serializer = CreateSerializer(json);

        // Act
        var result = serializer.Deserialize<TestModel>("main.pirate");

        // Assert
        Assert.Equal("Blackbeard", result.Name);
    }

    private static ObjectSerializer CreateSerializer(string json)
    {
        var logger = A.Fake<ILogger>();
        var environmentVariables = A.Fake<IEnvironmentVariables>();
        var fileWriteHandler = A.Fake<IFileWriteHandler>();
        var fileReadHandler = A.Fake<IFileReadHandler>();

        A.CallTo(() => environmentVariables.GetVariable("location")).Returns("/tmp/pirate-common-test");
        A.CallTo(() => fileReadHandler.FileExists(A<string>.Ignored, FileExtension.JSON, A<string>.Ignored)).Returns(true);
        A.CallTo(() => fileReadHandler.ReadAllTextFromFile(A<string>.Ignored, FileExtension.JSON, A<string>.Ignored))
            .Returns(Task.FromResult(json));

        return new ObjectSerializer(logger, environmentVariables, fileWriteHandler, fileReadHandler);
    }
}
