using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PirateLexer.Test
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
            var result = keyWordService.GetTokenControlKeywork("if");

            Assert.Equal(TokenType.IF, result);
        }

        [Fact]
        public void ShouldReturnElse()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("else");

            Assert.Equal(TokenType.ELSE, result);
        }

        [Fact]
        public void ShouldReturnFor()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("for");

            Assert.Equal(TokenType.FOR, result);
        }

        [Fact]
        public void ShouldReturnTo()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("to");

            Assert.Equal(TokenType.TO, result);
        }

        [Fact]
        public void ShouldReturnForeach()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("foreach");

            Assert.Equal(TokenType.FOREACH, result);
        }

        [Fact]
        public void ShouldReturnIn()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("in");

            Assert.Equal(TokenType.IN, result);
        }

        [Fact]
        public void ShouldReturnWhile()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("while");

            Assert.Equal(TokenType.WHILE, result);
        }

        [Fact]
        public void ShouldReturnFunc()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("func");

            Assert.Equal(TokenType.FUNC, result);
        }

        [Fact]
        public void ShouldReturnClass()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("class");

            Assert.Equal(TokenType.CLASS, result);
        }

        [Fact]
        public void ShouldReturnNew()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("new");

            Assert.Equal(TokenType.NEW, result);
        }

        [Fact]
        public void ShouldReturnVoid()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("void");

            Assert.Equal(TokenType.VOID, result);
        }


    }
}