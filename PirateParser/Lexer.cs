
using PirateParser.Models;

namespace PirateParser;
class Lexer
{
    public string fileName { get; set; }
    public static string text { get; set; }
    public static char currentChar { get; set; }
    public static Position position { get; set; }

    public Lexer(string FileName, string[] Text)
    {
        fileName = FileName;
        foreach (var item in Text)
        {
            item.Replace('\t', ' ');
            item.Replace(' ');
            text += item;
        }
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

        while (currentChar != null)
        {
            if (currentChar.Equals('\r'))
            {
                Advance();
            }
            if (currentChar.Equals('\n'))
            {
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
                tokens.Add(TokenRepository.MakeNumber());
                continue;
            }
            if (Globals.LETTERS.Contains(currentChar))
            {
                tokens.Add(TokenRepository.MakeIdentifier());
                continue;
            }
            switch (currentChar)
            {
                case '"':
                    tokens.Add(TokenRepository.MakeString());
                    continue;
                case '+':
                    tokens.Add(new Token(
                        TokenType.PLUS,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '-':
                    tokens.Add(new Token(
                        TokenType.MINUS,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '*':
                    tokens.Add(new Token(
                        TokenType.MULTIPLY,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '/':
                    tokens.Add(new Token(
                        TokenType.DIVIDE,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '^':
                    tokens.Add(new Token(
                        TokenType.POWER,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '(':
                    tokens.Add(new Token(
                        TokenType.LEFTPARENTHESES,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ')':
                    tokens.Add(new Token(
                        TokenType.RIGHTPARENTHESES,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '{':
                    tokens.Add(new Token(
                        TokenType.LEFTCURLYBRACE,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '}':
                    tokens.Add(new Token(
                        TokenType.RIGHTCURLYBRACE,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ',':
                    tokens.Add(new Token(
                        TokenType.COMMA,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case ':':
                    tokens.Add(new Token(
                        TokenType.COLON,
                        PositionStart: position
                    ));
                    Advance();
                    continue;
                case '=':
                    tokens.Add(TokenRepository.MakeEquals());
                    Advance();
                    continue;
                case '<':
                    tokens.Add(TokenRepository.MakeLessThan());
                    Advance();
                    continue;
                case '>':
                    tokens.Add(TokenRepository.MakeGreaterThan());
                    Advance();
                    continue;
                case '!':
                    var result = TokenRepository.MakeNotEquals();
                    if (result.error != null)
                    {
                        return (null, result.error);
                    }
                    Advance();
                    continue;
                default:
                    var positionStart = position.Copy();
                    Advance();
                    return (null, new Error(positionStart, position, $"'{currentChar}'", "Illegal Character"));
            }
        }

        tokens.Add(new Token(
            TokenType.ENDOFFILE,
            PositionStart: position
        ));
        return (tokens, null);
    }
}
