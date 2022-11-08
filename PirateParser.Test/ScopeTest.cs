using PirateParser.Node.Interfaces;

namespace PirateParser.Test;

public class ScopeTest
{
    [Fact]
    public void ShouldAddNode()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var scope = new Scope(logger);
        var node = A.Fake<INode>();

        // Act
        var result = scope.AddNode(node);

        // Assert
        Assert.True(result);
    }
}