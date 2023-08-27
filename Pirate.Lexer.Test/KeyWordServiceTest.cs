using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Pirate.Lexer.Test
{
    public class KeyWordServiceTest
    {
        [Fact]
        public void ShouldReturnVar()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("var");

            Assert.Equal(TokenType.VAR, result);
        }

        [Fact]
        public void ShouldReturnInt()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("int");

            Assert.Equal(TokenType.INT, result);
        }

        [Fact]
        public void ShouldReturnFloat()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("float");

            Assert.Equal(TokenType.FLOAT, result);
        }

        [Fact]
        public void ShouldReturnString()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("string");

            Assert.Equal(TokenType.STRING, result);
        }

        [Fact]
        public void ShouldReturnChar()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("char");

            Assert.Equal(TokenType.CHAR, result);
        }

        [Fact]
        public void ShouldReturnIf()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("if");

            Assert.Equal(TokenType.IF, result);
        }

        [Fact]
        public void ShouldReturnElse()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("else");

            Assert.Equal(TokenType.ELSE, result);
        }

        [Fact]
        public void ShouldReturnFor()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("for");

            Assert.Equal(TokenType.FOR, result);
        }

        [Fact]
        public void ShouldReturnTo()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("to");

            Assert.Equal(TokenType.TO, result);
        }

        [Fact]
        public void ShouldReturnForeach()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("foreach");

            Assert.Equal(TokenType.FOREACH, result);
        }

        [Fact]
        public void ShouldReturnIn()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("in");

            Assert.Equal(TokenType.IN, result);
        }

        [Fact]
        public void ShouldReturnWhile()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("while");

            Assert.Equal(TokenType.WHILE, result);
        }

        [Fact]
        public void ShouldReturnFunc()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("func");

            Assert.Equal(TokenType.FUNC, result);
        }

        [Fact]
        public void ShouldReturnClass()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("class");

            Assert.Equal(TokenType.CLASS, result);
        }

        [Fact]
        public void ShouldReturnNew()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("new");

            Assert.Equal(TokenType.NEW, result);
        }

        [Fact]
        public void ShouldReturnVoid()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("void");

            Assert.Equal(TokenType.VOID, result);
        }

        [Fact]
        public void ShouldReturnTrue()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeyword("return");

            Assert.Equal(TokenType.RETURN, result);
        }
    }
}