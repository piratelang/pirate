using Lexer.Tokens;
using Lexer.Enums;

namespace Lexer;

public sealed class Lexer : ILexer
{
    private static Lexer? lexer;
    public ILogger Logger { get; set; }

    public string fileName { get; set; }
    public string text { get; set; }
    public int position { get; set; } = 0;

    private Lexer(ILogger logger)
    {
        Logger = logger;
        logger.Log("Created Lexer", this.GetType().Name, LogType.INFO);
    }

    public static Lexer Instance(ILogger logger)
    {
        if (lexer == null)
        {
            lexer = new Lexer(logger);
        }
        return lexer;
    }

    public List<Token> MakeTokens(string Text, string FileName)
    {
        text = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        fileName = FileName;
        position = 0;

        List<Token> tokens = new();
        var result = true;

        while (result)
        {
            if (position + 1 == text.Length)
                if (text[position] == ' ')
                {
                    position += 1;
                    continue;
                }
            if (Char.IsDigit(text[position]))
            {
                tokens.Add(TokenRepository.MakeNumber(Logger));
                continue;
            }
            if (Char.IsLetter(text[position]))
            {
                tokens.Add(TokenRepository.MakeIdentifier(Logger));
                continue;
            }
            switch (text[position])
            {
                case '"':
                    tokens.Add(TokenRepository.MakeString(Logger));
                    continue;
                case '\'':
                    tokens.Add(TokenRepository.MakeChar(Logger));
                    continue;
                case '+':
                    tokens.Add(TokenRepository.MakePlus(Logger));
                    continue;
                case '-':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MINUS, Logger));
                    position += 1;
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MULTIPLY, Logger));
                    position += 1;
                    continue;
                case '/':
                    tokens.Add(TokenRepository.MakeDivide(Logger));
                    continue;
                case '^':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.POWER, Logger));
                    position += 1;
                    continue;
                case '(':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTPARENTHESES, Logger));
                    position += 1;
                    continue;
                case ')':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTPARENTHESES, Logger));
                    position += 1;
                    continue;
                case '{':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE, Logger));
                    position += 1;
                    continue;
                case '}':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE, Logger));
                    position += 1;
                    continue;
                case ',':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COMMA, Logger));
                    position += 1;
                    continue;
                case ':':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COLON, Logger));
                    position += 1;
                    continue;
                case ';':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.SEMICOLON, Logger));
                    position += 1;
                    continue;
                case '.':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOT, Logger));
                    position += 1;
                    continue;
                case '$':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOLLAR, Logger));
                    position += 1;
                    continue;
                case '[':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTBRACKET, Logger));
                    position += 1;
                    continue;
                case ']':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTBRACKET, Logger));
                    position += 1;
                    continue;
                case '=':
                    tokens.Add(TokenRepository.MakeEquals(Logger));
                    continue;
                case '<':
                    tokens.Add(TokenRepository.MakeLessThan(Logger));
                    continue;
                case '>':
                    tokens.Add(TokenRepository.MakeGreaterThan(Logger));
                    continue;
                case '!':
                    tokens.Add(TokenRepository.MakeNotEquals(Logger));
                    continue;
            }
        }
        return tokens;
    }
}
