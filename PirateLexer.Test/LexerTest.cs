using Xunit;
using AutoFixture;
using Common;
using AutoFixture.Kernel;
using Common.FileHandlers.Interfaces;
using Common.FileHandlers;

namespace PirateLexer.Test
{
    public class LexerTest
    {
        [Fact]
        public void ShouldMakeNumber()
        {
            //Arrange
            Fixture fixture = new();
            fixture.Customizations.Add(
                new TypeRelay(
                    typeof(IFileWriteHandler),
                    typeof(FileWriteHandler)
                )
            );
            var logger = fixture.Create<Logger>();
            var tokenRepository = fixture.Create<TokenRepository>();;
            
            Lexer lexer = new(logger, tokenRepository);

            //Act
            var result = lexer.MakeTokens("123", " ");

            //Assert
            Assert.Equal(1, result.Count);
        }
    }
}