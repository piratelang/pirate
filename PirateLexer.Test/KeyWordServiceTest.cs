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

            Assert.Equal(TokenTypeKeyword.VAR, result);
        }

        [Fact]
        public void ShouldReturnInt()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("int");

            Assert.Equal(TokenTypeKeyword.INT, result);
        }

        [Fact]
        public void ShouldReturnFloat()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("float");

            Assert.Equal(TokenTypeKeyword.FLOAT, result);
        }

        [Fact]
        public void ShouldReturnString()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("string");

            Assert.Equal(TokenTypeKeyword.STRING, result);
        }

        [Fact]
        public void ShouldReturnChar()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("char");

            Assert.Equal(TokenTypeKeyword.CHAR, result);
        }

        [Fact]
        public void ShouldReturnIf()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("if");

            Assert.Equal(TokenControlKeyword.IF, result);
        }

        [Fact]
        public void ShouldReturnElse()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("else");

            Assert.Equal(TokenControlKeyword.ELSE, result);
        }

        [Fact]
        public void ShouldReturnFor()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("for");

            Assert.Equal(TokenControlKeyword.FOR, result);
        }

        [Fact]
        public void ShouldReturnTo()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("to");

            Assert.Equal(TokenControlKeyword.TO, result);
        }

        [Fact]
        public void ShouldReturnForeach()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("foreach");

            Assert.Equal(TokenControlKeyword.FOREACH, result);
        }

        [Fact]
        public void ShouldReturnIn()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("in");

            Assert.Equal(TokenControlKeyword.IN, result);
        }

        [Fact]
        public void ShouldReturnWhile()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("while");

            Assert.Equal(TokenControlKeyword.WHILE, result);
        }

        [Fact]
        public void ShouldReturnFunc()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("func");

            Assert.Equal(TokenControlKeyword.FUNC, result);
        }

        [Fact]
        public void ShouldReturnClass()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("class");

            Assert.Equal(TokenControlKeyword.CLASS, result);
        }

        [Fact]
        public void ShouldReturnNew()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTokenControlKeywork("new");

            Assert.Equal(TokenControlKeyword.NEW, result);
        }

        [Fact]
        public void ShouldReturnVoid()
        {
            var keyWordService = new KeyWordService();
            var result = keyWordService.GetTypeKeyword("void");

            Assert.Equal(TokenTypeKeyword.VOID, result);
        }


    }
}