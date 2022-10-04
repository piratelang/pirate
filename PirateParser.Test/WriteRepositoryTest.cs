using Xunit;

using System.IO;
using PirateLexer;

namespace PirateParser.Test;

public class WriteRepositoryTest
{
    [Fact]
    public void ShouldReturnForLoop()
    {
        //Arrange
        var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForLoop.pirate");
        var lexer = new Lexer("test", text);
        var tokenList = lexer.MakeTokens();
        
        //Act
        var parser = new Parser(tokenList.tokens);
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
        var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForeachLoop.pirate");
        var lexer = new Lexer("test", text);
        var tokenList = lexer.MakeTokens();

        //Act
        var parser = new Parser(tokenList.tokens);
        var result = parser.Parse("output", "ShouldReturnForeachLoop");

        //Assert
        var filetest = File.ReadAllText("./output/ShouldReturnForeachLoop.py");
        Assert.True(
            File.ReadAllText("./output/ShouldReturnForeachLoop.py") == "for item in list :\n    "
        );
    }
}