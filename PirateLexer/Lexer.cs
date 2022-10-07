
using Common;
using Common.Enum;
using PirateLexer.Models;

namespace PirateLexer;
public class Lexer
{
    public string fileName { get; set; }
    public static string text { get; set; }
    public static char currentChar { get; set; }
    public static Position position { get; set; }
    public Logger logger { get; set; }

    public Lexer(string FileName, string Text, Logger Logger)
    {
        logger = Logger;
        fileName = FileName;
        var newText = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        text = newText;
        position = new Position(-1, 0, -1, fileName, text);
        Advance();
    }

    public static void Advance()
    {
        position.Advance(currentChar);
        if (position.index < text.Length)
        {
            currentChar = text[position.index];
        }
        else
        {
            currentChar = ' ';
        }
    }

    public (List<Token>? tokens, Error? error) MakeTokens()
    {
        List<Token> tokens = new List<Token> { };

        logger.Log("Starting Lexing text", "Lexer", LogType.INFO);
        while (currentChar != null)
        {
            if (currentChar.Equals('\n'))
            {
                tokens.Add(new Token(
                    TokenType.ENDOFLINE,
                    logger,
                    PositionStart: position
                ));
                Advance();
            }
            if (currentChar == ' ')
            {
                Advance();
                if (currentChar == ' ')
                {
                    break;
                }
                continue;
            }
            if (Globals.DIGITS.Contains(currentChar))
            {
                tokens.Add(TokenRepository.MakeNumber(logger));
                continue;
            }
            if (Globals.LETTERS.Contains(currentChar))
            {
                tokens.Add(TokenRepository.MakeIdentifier(logger));
                continue;
            }
            switch (currentChar)
            {
                case '"':
                    tokens.Add(TokenRepository.MakeString(logger));
                    continue;
                case '+':
                    tokens.Add(TokenRepository.MakePlus(logger));
                    Advance();
                    continue;
                case '-':
                    tokens.Add(new Token(
                        TokenType.MINUS,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '*':
                    tokens.Add(new Token(
                        TokenType.MULTIPLY,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '/':
                    tokens.Add(TokenRepository.MakeDivide(logger));
                    Advance();
                    continue;
                case '^':
                    tokens.Add(new Token(
                        TokenType.POWER,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '(':
                    tokens.Add(new Token(
                        TokenType.LEFTPARENTHESES,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ')':
                    tokens.Add(new Token(
                        TokenType.RIGHTPARENTHESES,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '{':
                    tokens.Add(new Token(
                        TokenType.LEFTCURLYBRACE,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '}':
                    tokens.Add(new Token(
                        TokenType.RIGHTCURLYBRACE,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ',':
                    tokens.Add(new Token(
                        TokenType.COMMA,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ':':
                    tokens.Add(new Token(
                        TokenType.COLON,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ';':
                    tokens.Add(new Token(
                        TokenType.SEMICOLON,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '.':
                    tokens.Add(new Token(
                        TokenType.DOT,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '$':
                    tokens.Add(new Token(
                        TokenType.DOLLAR,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '[':
                    tokens.Add(new Token(
                        TokenType.LEFTBRACKET,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ']':
                    tokens.Add(new Token(
                        TokenType.RIGHTBRACKET,
                        logger,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '=':
                    tokens.Add(TokenRepository.MakeEquals(logger));
                    Advance();
                    continue;
                case '<':
                    tokens.Add(TokenRepository.MakeLessThan(logger));
                    Advance();
                    continue;
                case '>':
                    tokens.Add(TokenRepository.MakeGreaterThan(logger));
                    Advance();
                    continue;
                case '!':
                    var result = TokenRepository.MakeNotEquals(logger);
                    if (result.error != null)
                    {
                        return (null, result.error);
                    }
                    tokens.Add(result.token);
                    Advance();
                    continue;
                default:
                    var positionStart = position.Copy();
                    Advance();
                    logger.Log($"{currentChar} not found in the Lexer", "Lexer", LogType.ERROR);
                    return (null, new Error(positionStart, position, $"'{currentChar}'", "Illegal Character"));
            }
        }
        logger.Log("Finished Lexing\n", "Lexer", LogType.INFO);
        return (tokens, null);
    }
}
