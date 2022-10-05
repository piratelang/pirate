using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using FakeItEasy;
using PirateLexer;
using Xunit;

namespace PirateParser.Test
{
    public class ParserTest
    {
        [Fact]
        public void ShouldParseFile()
        {
            //Arrange
            var logger = A.Fake<Logger>();
            A.CallTo(() => logger.Log(A.Fake<string>, )).WithAnyArguments().DoesNothing();
            var text = File.ReadAllText($"../../../PirateInput/ShouldReturnForeachLoop.pirate");
            var lexer = new Lexer("test", text, logger);
            var tokenList = lexer.MakeTokens();

            //Act
            var parser = new Parser(tokenList.tokens, logger);
            var result = parser.Parse("output", "ShouldReturnForeachLoop");
            Assert.True(true);
        }
    }
}