using Xunit;

using System.IO;
using PirateLexer;
using Common;
using FakeItEasy;

namespace PirateParser.Test;

public class WriteRepositoryTest
{
    [Fact]
    public void ShouldReturnForLoop()
    {
        //Arrange
        var logger = A.Fake<Logger>();
        A.CallTo(() => logger.Log).WithAnyArguments().DoesNothing();
        var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForLoop.pirate");
        var lexer = new Lexer("test", text, logger);
        var tokenList = lexer.MakeTokens();
        
        //Act
        var parser = new Parser(tokenList.tokens, logger);
        var result = parser.Parse("output", "ShouldReturnForLoop");

        //Assert
        Assert.True(
            File.ReadAllText("./output/ShouldReturnForLoop.py") == "for i in range(0, 4) :\n    "
        );
    }

    [Fact]
    public void ShouldReturnForeachLoop()
    {
        //Arrange
        var logger = A.Fake<Logger>();
        A.CallTo(() => logger.Log).WithAnyArguments().DoesNothing();
        var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForeachLoop.pirate");
        var lexer = new Lexer("test", text, logger);
        var tokenList = lexer.MakeTokens();

        //Act
        var parser = new Parser(tokenList.tokens, logger);
        var result = parser.Parse("output", "ShouldReturnForeachLoop");

        //Assert
        Assert.True(
            File.ReadAllText("./output/ShouldReturnForeachLoop.py") == "for item in list :\n    "
        );
    }
}