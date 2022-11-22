using PirateLexer.Tokens;
using PirateLexer.Enums;
using PirateLexer.Interfaces;

namespace PirateLexer;

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
        logger.Log("Created Lexer", this.GetType().Name, LogType.INFO);
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
            if (Char.IsDigit(text[position]))
            {
                var tokenResult = _tokenRepository.MakeNumber(text, position);
                tokens.Add(tokenResult.Token);
                position = tokenResult.Position;
                Logger.Log($"Creating Token \"{tokenResult.Token.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
                continue;
            }
            if (Char.IsLetter(text[position]))
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
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MINUS, Logger));
                    position += 1;
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MULTIPLY, Logger));
                    position += 1;
                    continue;
                case '/':
                    tokenResult = _tokenRepository.MakeDivide(text, position);
                    tokens.Add(tokenResult.Token);
                    position = tokenResult.Position;
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
            }
        }
        return tokens;
    }
}
