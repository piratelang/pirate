using Pirate.Lexer;
using Xunit;

namespace Pirate.Lexer.Test;

public class LexerTests
{
    private static TokenType[] TypesOf(LexResult result) =>
        result.Tokens.Select(t => t.Type).ToArray();

    [Fact]
    public void Tokenize_EmptySource_ReturnsOnlyEof()
    {
        var result = Lexer.Tokenize(string.Empty);

        Assert.Equal([TokenType.Eof], TypesOf(result));
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Tokenize_WhitespaceOnly_ReturnsOnlyEof()
    {
        var result = Lexer.Tokenize("  \t\r\n \n ");

        Assert.Equal([TokenType.Eof], TypesOf(result));
        Assert.Empty(result.Diagnostics);
    }

    [Theory]
    [InlineData("var", TokenType.Var)]
    [InlineData("int", TokenType.Int)]
    [InlineData("float", TokenType.Float)]
    [InlineData("string", TokenType.String)]
    [InlineData("char", TokenType.Char)]
    [InlineData("bool", TokenType.Bool)]
    [InlineData("void", TokenType.Void)]
    [InlineData("func", TokenType.Func)]
    [InlineData("if", TokenType.If)]
    [InlineData("else", TokenType.Else)]
    [InlineData("while", TokenType.While)]
    [InlineData("for", TokenType.For)]
    [InlineData("in", TokenType.In)]
    [InlineData("to", TokenType.To)]
    [InlineData("return", TokenType.Return)]
    [InlineData("extern", TokenType.Extern)]
    [InlineData("true", TokenType.True)]
    [InlineData("false", TokenType.False)]
    [InlineData("class", TokenType.Class)]
    [InlineData("new", TokenType.New)]
    public void Tokenize_Keyword_ProducesKeywordToken(string source, TokenType expected)
    {
        var result = Lexer.Tokenize(source);

        Assert.Equal([expected, TokenType.Eof], TypesOf(result));
    }

    [Fact]
    public void Tokenize_TrueLiteral_HasBoolValue()
    {
        var result = Lexer.Tokenize("true");

        Assert.Equal(true, result.Tokens[0].Value);
    }

    [Fact]
    public void Tokenize_FalseLiteral_HasBoolValue()
    {
        var result = Lexer.Tokenize("false");

        Assert.Equal(false, result.Tokens[0].Value);
    }

    [Fact]
    public void Tokenize_Identifier_ProducesIdentifierTokenWithLexeme()
    {
        var result = Lexer.Tokenize("counter_1");

        var token = result.Tokens[0];
        Assert.Equal(TokenType.Identifier, token.Type);
        Assert.Equal("counter_1", token.Lexeme);
        Assert.Null(token.Value);
    }

    [Fact]
    public void Tokenize_IdentifierWithKeywordPrefix_IsNotTreatedAsKeyword()
    {
        var result = Lexer.Tokenize("intValue");

        Assert.Equal([TokenType.Identifier, TokenType.Eof], TypesOf(result));
    }

    [Fact]
    public void Tokenize_QualifiedName_ProducesDotSeparatedIdentifiers()
    {
        var result = Lexer.Tokenize("Standard.Terminal.Print");

        Assert.Equal(
            [TokenType.Identifier, TokenType.Dot, TokenType.Identifier, TokenType.Dot, TokenType.Identifier, TokenType.Eof],
            TypesOf(result));
    }

    [Fact]
    public void Tokenize_IntLiteral_HasIntValue()
    {
        var result = Lexer.Tokenize("42");

        var token = result.Tokens[0];
        Assert.Equal(TokenType.IntLiteral, token.Type);
        Assert.Equal(42, token.Value);
    }

    [Fact]
    public void Tokenize_FloatLiteral_HasDoubleValue()
    {
        var result = Lexer.Tokenize("3.14");

        var token = result.Tokens[0];
        Assert.Equal(TokenType.FloatLiteral, token.Type);
        Assert.Equal(3.14, token.Value);
    }

    [Fact]
    public void Tokenize_IntFollowedByDot_DoesNotConsumeDotWithoutFractionalDigits()
    {
        var result = Lexer.Tokenize("3.length");

        Assert.Equal(
            [TokenType.IntLiteral, TokenType.Dot, TokenType.Identifier, TokenType.Eof],
            TypesOf(result));
    }

    [Fact]
    public void Tokenize_IntLiteralOverflow_ReportsDiagnostic()
    {
        var result = Lexer.Tokenize("99999999999999999999");

        Assert.Single(result.Diagnostics);
        Assert.Contains("out of range", result.Diagnostics[0].Message);
    }

    [Fact]
    public void Tokenize_StringLiteral_DecodesContentAsValue()
    {
        var result = Lexer.Tokenize("\"Ahoy!\"");

        var token = result.Tokens[0];
        Assert.Equal(TokenType.StringLiteral, token.Type);
        Assert.Equal("Ahoy!", token.Value);
    }

    [Theory]
    [InlineData("\"a\\nb\"", "a\nb")]
    [InlineData("\"a\\tb\"", "a\tb")]
    [InlineData("\"say \\\"hi\\\"\"", "say \"hi\"")]
    [InlineData("\"back\\\\slash\"", "back\\slash")]
    public void Tokenize_StringLiteral_DecodesKnownEscapeSequences(string source, string expected)
    {
        var result = Lexer.Tokenize(source);

        Assert.Equal(expected, result.Tokens[0].Value);
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Tokenize_StringLiteral_UnknownEscapeSequence_ReportsDiagnosticAndKeepsCharacter()
    {
        var result = Lexer.Tokenize("\"a\\qb\"");

        Assert.Single(result.Diagnostics);
        Assert.Contains("Unknown escape sequence", result.Diagnostics[0].Message);
        Assert.Equal("aqb", result.Tokens[0].Value);
    }

    [Fact]
    public void Tokenize_UnterminatedStringLiteral_ReportsDiagnostic()
    {
        var result = Lexer.Tokenize("\"never closed");

        Assert.Single(result.Diagnostics);
        Assert.Contains("Unterminated string literal", result.Diagnostics[0].Message);
    }

    [Fact]
    public void Tokenize_CharLiteral_HasCharValue()
    {
        var result = Lexer.Tokenize("'x'");

        var token = result.Tokens[0];
        Assert.Equal(TokenType.CharLiteral, token.Type);
        Assert.Equal('x', token.Value);
    }

    [Fact]
    public void Tokenize_EscapedCharLiteral_HasDecodedValue()
    {
        var result = Lexer.Tokenize("'\\n'");

        Assert.Equal('\n', result.Tokens[0].Value);
    }

    [Fact]
    public void Tokenize_UnterminatedCharLiteral_ReportsDiagnostic()
    {
        var result = Lexer.Tokenize("'x");

        Assert.Single(result.Diagnostics);
        Assert.Contains("Unterminated char literal", result.Diagnostics[0].Message);
    }

    [Theory]
    [InlineData("=", TokenType.Equal)]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Star)]
    [InlineData("/", TokenType.Slash)]
    [InlineData("%", TokenType.Percent)]
    [InlineData("^", TokenType.Caret)]
    [InlineData("==", TokenType.EqualEqual)]
    [InlineData("!=", TokenType.BangEqual)]
    [InlineData("<", TokenType.Less)]
    [InlineData("<=", TokenType.LessEqual)]
    [InlineData(">", TokenType.Greater)]
    [InlineData(">=", TokenType.GreaterEqual)]
    [InlineData("&&", TokenType.AmpAmp)]
    [InlineData("||", TokenType.PipePipe)]
    [InlineData("!", TokenType.Bang)]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("{", TokenType.LeftBrace)]
    [InlineData("}", TokenType.RightBrace)]
    [InlineData("[", TokenType.LeftBracket)]
    [InlineData("]", TokenType.RightBracket)]
    [InlineData(",", TokenType.Comma)]
    [InlineData(":", TokenType.Colon)]
    [InlineData(";", TokenType.Semicolon)]
    [InlineData(".", TokenType.Dot)]
    public void Tokenize_Operator_ProducesExpectedToken(string source, TokenType expected)
    {
        var result = Lexer.Tokenize(source);

        Assert.Equal([expected, TokenType.Eof], TypesOf(result));
        Assert.Empty(result.Diagnostics);
    }

    [Fact]
    public void Tokenize_LoneAmpersand_ReportsDiagnosticInsteadOfToken()
    {
        var result = Lexer.Tokenize("&");

        Assert.Equal([TokenType.Eof], TypesOf(result));
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void Tokenize_LonePipe_ReportsDiagnosticInsteadOfToken()
    {
        var result = Lexer.Tokenize("|");

        Assert.Equal([TokenType.Eof], TypesOf(result));
        Assert.Single(result.Diagnostics);
    }

    [Fact]
    public void Tokenize_UnknownCharacter_ReportsDiagnosticAndSkipsIt()
    {
        var result = Lexer.Tokenize("@");

        Assert.Equal([TokenType.Eof], TypesOf(result));
        Assert.Single(result.Diagnostics);
        Assert.Contains("Unexpected character '@'", result.Diagnostics[0].Message);
    }

    [Fact]
    public void Tokenize_LineComment_IsStrippedFromTokenStream()
    {
        var result = Lexer.Tokenize("1 // this is ignored\n2");

        Assert.Equal([TokenType.IntLiteral, TokenType.IntLiteral, TokenType.Eof], TypesOf(result));
        Assert.Equal(1, result.Tokens[0].Value);
        Assert.Equal(2, result.Tokens[1].Value);
    }

    [Fact]
    public void Tokenize_LineCommentAtEndOfFile_NoTrailingNewline_IsStripped()
    {
        var result = Lexer.Tokenize("1 // trailing, no newline");

        Assert.Equal([TokenType.IntLiteral, TokenType.Eof], TypesOf(result));
    }

    [Fact]
    public void Tokenize_SingleDivide_IsNotConfusedWithComment()
    {
        var result = Lexer.Tokenize("6 / 2");

        Assert.Equal([TokenType.IntLiteral, TokenType.Slash, TokenType.IntLiteral, TokenType.Eof], TypesOf(result));
    }

    [Fact]
    public void Tokenize_SingleLine_TracksColumns()
    {
        var result = Lexer.Tokenize("ab + 1");

        Assert.Equal(1, result.Tokens[0].Location.Line);
        Assert.Equal(1, result.Tokens[0].Location.Column); // "ab"
        Assert.Equal(4, result.Tokens[1].Location.Column); // "+"
        Assert.Equal(6, result.Tokens[2].Location.Column); // "1"
    }

    [Fact]
    public void Tokenize_MultipleLines_TracksLineAndResetsColumn()
    {
        var result = Lexer.Tokenize("a\nbb");

        Assert.Equal(1, result.Tokens[0].Location.Line);
        Assert.Equal(1, result.Tokens[0].Location.Column);

        Assert.Equal(2, result.Tokens[1].Location.Line);
        Assert.Equal(1, result.Tokens[1].Location.Column);
    }

    [Fact]
    public void Tokenize_FunctionDeclaration_ProducesExpectedTokenStream()
    {
        const string source = """
            extern Standard.Terminal.Print;

            func main() : void
            {
                if 3 == 3
                {
                    Standard.Terminal.Print("Ahoy!");
                }
            }
            """;

        var result = Lexer.Tokenize(source);

        Assert.Empty(result.Diagnostics);
        Assert.Equal(TokenType.Extern, result.Tokens[0].Type);
        Assert.Equal(TokenType.Eof, result.Tokens[^1].Type);
        Assert.Contains(result.Tokens, t => t.Type == TokenType.StringLiteral && (string)t.Value! == "Ahoy!");
    }
}
