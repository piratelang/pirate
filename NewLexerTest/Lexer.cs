
using NewPirateLexer.Tokens;
using NewPirateLexer.Enums;

namespace NewPirateLexer;
public class Lexer
{
    public string fileName { get; set; }
    public static string text { get; set; }
    public static char currentChar { get; set; }
    public static int position { get; set; }

    public Lexer(string FileName, string Text)
    {
        fileName = FileName;
        position = -1;
        text = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        Advance();
    }

    public static void Advance()
    {
        position += 1;
        if (position + 1 < text.Length)
        {
            currentChar = text[position];
        }
        else
        {
            currentChar = '€';
        }
    }

    public (List<Token>? tokens, Error? error) MakeTokens()
    {
        List<Token> tokens = new List<Token> { };

        while (currentChar != '€')
        {
            if (currentChar == ' ')
            {
                Advance();
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
                case '\'':
                    tokens.Add(TokenRepository.MakeChar());
                    continue;
                case '+':
                    tokens.Add(TokenRepository.MakePlus());
                    Advance();
                    continue;
                case '-':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MINUS));
                    Advance();
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MULTIPLY));
                    Advance();
                    continue;
                case '/':
                    tokens.Add(TokenRepository.MakeDivide());
                    Advance();
                    continue;
                case '^':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.POWER));
                    Advance();
                    continue;
                case '(':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTPARENTHESES));
                    Advance();
                    continue;
                case ')':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTPARENTHESES));
                    Advance();
                    continue;
                case '{':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE));
                    Advance();
                    continue;
                case '}':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE));
                    Advance();
                    continue;
                case ',':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COMMA));
                    Advance();
                    continue;
                case ':':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COLON));
                    Advance();
                    continue;
                case ';':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.SEMICOLON));
                    Advance();
                    continue;
                case '.':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOT));
                    Advance();
                    continue;
                case '$':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOLLAR));
                    Advance();
                    continue;
                case '[':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTBRACKET));
                    Advance();
                    continue;
                case ']':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTBRACKET));
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
                    tokens.Add(result.token);
                    Advance();
                    continue;
                default:
                    Advance();
                    return (null, new Error($"'{currentChar}'", "Illegal Character"));
            }
        }
        return (tokens, null);
    }
}
