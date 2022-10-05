using System.IO;
using Common;
using Xunit;

namespace PirateLexer.Test;

public class LexerTest
{
    [Fact]
    public void ShouldReturnNextCharacter()
    {
        //Arrange
        var lexer = new Lexer("Test", "abc", new Logger("Test"));

        //Act
        Lexer.Advance();

        //Assert
        Assert.Equal('b', Lexer.currentChar);
    }

    [Fact]
    public void ShouldReturnNoCharacter()
    {
        //Arrange
        var lexer = new Lexer("Test", "abc", new Logger("Test"));

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
        // Arrange
        var text = File.ReadAllText($"../../../test.pirate");
        var lexer = new Lexer("test", text, new Logger("Test"));

        //Act
        var result = lexer.MakeTokens();
    
        // Then
        Assert.True(result.tokens.Count == 138);
    }
}