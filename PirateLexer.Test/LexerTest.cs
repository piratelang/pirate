using System.IO;
using Xunit;

namespace PirateLexer.Test;

public class LexerTest
{
    [Fact]
    public void ShouldReturnNextCharacter()
    {
        //Arrange
        var lexer = new Lexer("Test", "abc");

        //Act
        Lexer.Advance();

        //Assert
        Assert.Equal('b', Lexer.currentChar);
    }

    [Fact]
    public void ShouldReturnNoCharacter()
    {
        //Arrange
        var lexer = new Lexer("Test", "abc");

        //Act
        Lexer.Advance();
        Lexer.Advance();
        Lexer.Advance();
        
        //Assert
        Assert.Equal(' ', Lexer.currentChar);
    }

    [Fact]
    public void ShouldLexTokens()
    {   
        //Arrange
        string newPath = Path.GetFullPath(Path.Combine("./", @"..\..\..\"));
        var text = File.ReadAllText($"{newPath}/test.pirate");
        var lexer = new Lexer("test", text);

        //Act
        var result = lexer.MakeTokens();
    
        // Then
        Assert.True(result.tokens.Count == 138);
    }
}