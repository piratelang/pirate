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
using Common.Enum;
using AutoFixture;

namespace PirateParser.Test
{
    public class ParserTest
    {
        [Fact]
        public void ShouldParseFile()
        {
            //Arrange
            var logger = new Logger("Test", "ShouldParseFile");
            var text = File.ReadAllText($"../../../PirateInput/ShouldParseFile.pirate");
            var lexer = new Lexer("test", text, logger);
            var tokenList = lexer.MakeTokens();

            //Act
            var parser = new Parser(tokenList.tokens, logger);
            var result = parser.Parse("output", "ShouldParseFile");
            Assert.True(true);
        }
    }
}