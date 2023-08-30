using Pirate.Lexer.Enums;
using Pirate.Lexer.Tokens;
using Pirate.Lexer.Interfaces;

namespace Pirate.Lexer;

/// <summary>
/// A starting point for the lexer.
/// </summary>
public class Lexer : ILexer
{
    private static Lexer lexer;
    private readonly ITokenRepository _tokenRepository;

    public ILogger Logger { get; set; }

    public string fileName { get; set; }
    public string text { get; set; }
    public int position { get; set; } = 0;

    public Lexer(ILogger logger, ITokenRepository tokenRepository)
    {
        Logger = logger;
        _tokenRepository = tokenRepository;
        logger.Log("Created Lexer", LogType.INFO);
    }

    public List<Token> MakeTokens(string Text, string FileName)
    {
        text = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        if (text == null)
        {
            throw new NullReferenceException("Lexer text is null");
        }
        fileName = FileName;
        position = 0;

        List<Token> tokens = new();
        var result = true;

        while (result)
        {
            if (position >= text.Length)
            {
                break;
            }
            if (text[position] == ' ')
            {
                position += 1;
                continue;
            }
            if (char.IsDigit(text[position]))
            {
                var tokenResult = _tokenRepository.MakeNumber(text, position);
                tokens.Add(tokenResult.Token);
                position = tokenResult.Position;
                Logger.Log($"Creating Token \"{tokenResult.Token.ToString()}\"", LogType.INFO);
                continue;
            }
            if (char.IsLetter(text[position]))
            {
                var tokenResult = _tokenRepository.MakeIdentifier(text, position);
                tokens.Add(tokenResult.Token);
                position = tokenResult.Position;
                continue;
            }
            switch (text[position])
            {
                case '"':
                    var tokenResult = _tokenRepository.MakeString(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '\'':
                    tokenResult = _tokenRepository.MakeChar(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '+':
                    tokenResult = _tokenRepository.MakePlus(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '-':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenType.MINUS));
                    position += 1;
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenType.MULTIPLY));
                    position += 1;
                    continue;
                case '%':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenType.MODULO));
                    position += 1;
                    continue;
                case '/':
                    tokenResult = _tokenRepository.MakeDivide(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '^':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenType.POWER));
                    position += 1;
                    continue;
                case '(':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTPARENTHESES));
                    position += 1;
                    continue;
                case ')':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTPARENTHESES));
                    position += 1;
                    continue;
                case '{':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTCURLYBRACE));
                    position += 1;
                    continue;
                case '}':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTCURLYBRACE));
                    position += 1;
                    continue;
                case ',':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COMMA));
                    position += 1;
                    continue;
                case ':':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.COLON));
                    position += 1;
                    continue;
                case ';':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.SEMICOLON));
                    position += 1;
                    continue;
                case '.':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.DOT));
                    position += 1;
                    continue;
                case '$':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.DOLLAR));
                    position += 1;
                    continue;
                case '[':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.LEFTBRACKET));
                    position += 1;
                    continue;
                case ']':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenType.RIGHTBRACKET));
                    position += 1;
                    continue;
                case '=':
                    tokenResult = _tokenRepository.MakeEquals(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '<':
                    tokenResult = _tokenRepository.MakeLessThan(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '>':
                    tokenResult = _tokenRepository.MakeGreaterThan(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                case '!':
                    tokenResult = _tokenRepository.MakeNotEquals(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
                    continue;
                default:
                    tokens.Add(new Token(TokenGroup.Empty, TokenType.Empty, text[position].ToString()));
                    position += 1;
                    Logger.Log($"Creating Unknown Token \"{text[position].ToString()}\"", LogType.WARNING);
                    continue;
            }

        }
        return tokens;
    }
}
