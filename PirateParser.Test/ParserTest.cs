using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PirateLexer;
using Xunit;

namespace PirateParser.Test
{
    public class ParserTest
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForeachLoop.pirate");
            var lexer = new Lexer("test", text);
            var tokenList = lexer.MakeTokens();

            //Act
            var parser = new Parser(tokenList.tokens);
            var result = parser.Parse("output", "ShouldReturnForeachLoop");
            Assert.True(true);
        }
    }
}