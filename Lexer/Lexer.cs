using Lexer.Tokens;
using Lexer.Enums;
using Common;
using Common.Enum;

namespace Lexer;

public class Lexer : ILexer
{
    public ILogger Logger { get; set; }

    public string fileName { get; set; }
    public static string text { get; set; }
    public static char currentChar { get; set; }
    public static int position { get; set; }

    public Lexer(ILogger logger)
    {
        Logger = logger;
        position = -1;

        logger.Log("Created Lexer", this.GetType().Name, LogType.INFO);
    }

    public static void Advance()
    {
        position += 1;
        if (position + 1 <= text.Length)
        {
            currentChar = text[position];
        }
        else
        {
            currentChar = '€';
        }
    }

    public (List<Token>? tokens, Error? error) MakeTokens(string Text, string FileName)
    {
        text = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        fileName = FileName;

        Advance();
        
        List<Token> tokens = new();

        while (currentChar != '€')
        {
            if (currentChar == ' ')
            {
                Advance();
                continue;
            }
            if (Globals.DIGITS.Contains(currentChar))
            {
                tokens.Add(TokenRepository.MakeNumber(Logger));
                continue;
            }
            if (Globals.LETTERS.Contains(currentChar))
            {
                tokens.Add(TokenRepository.MakeIdentifier(Logger));
                continue;
            }
            switch (currentChar)
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
                    Advance();
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MULTIPLY, Logger));
                    Advance();
                    continue;
                case '/':
                    tokens.Add(TokenRepository.MakeDivide(Logger));
                    continue;
                case '^':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.POWER, Logger));
                    Advance();
                    continue;
                case '(':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTPARENTHESES, Logger));
                    Advance();
                    continue;
                case ')':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTPARENTHESES, Logger));
                    Advance();
                    continue;
                case '{':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE, Logger));
                    Advance();
                    continue;
                case '}':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE, Logger));
                    Advance();
                    continue;
                case ',':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COMMA, Logger));
                    Advance();
                    continue;
                case ':':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COLON, Logger));
                    Advance();
                    continue;
                case ';':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.SEMICOLON, Logger));
                    Advance();
                    continue;
                case '.':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOT, Logger));
                    Advance();
                    continue;
                case '$':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOLLAR, Logger));
                    Advance();
                    continue;
                case '[':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTBRACKET, Logger));
                    Advance();
                    continue;
                case ']':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTBRACKET, Logger));
                    Advance();
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
                    var result = TokenRepository.MakeNotEquals(Logger);
                    if (result.error != null)
                    {
                        return (null, result.error);
                    }
                    tokens.Add(result.token);
                    continue;
                default:
                    Advance();
                    return (null, new Error($"'{currentChar}'", "Illegal Character"));
            }
        }
        return (tokens, null);
    }
}
