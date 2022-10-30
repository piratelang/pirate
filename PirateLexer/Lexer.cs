using PirateLexer.Tokens;
using PirateLexer.Enums;

namespace PirateLexer;

public sealed class Lexer : ILexer
{
    private static Lexer? lexer;
    public ILogger Logger { get; set; }

    public string fileName { get; set; }
    public string text { get; set; }
    public int position { get; set; } = 0;

    public Lexer(ILogger logger)
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
        Instance(Logger);
        lexer.text = Text.Replace("\n", "").Replace("\r", "").Replace("    ", "");
        if (lexer.text == null)
        {
            throw new NullReferenceException("Lexer text is null");
        }
        lexer.fileName = FileName;
        lexer.position = 0;

        List<Token> tokens = new();
        var result = true;

        while (result)
        {
            if (lexer.position >= lexer.text.Length)
            {
                break;
            }
            if (lexer.text[lexer.position] == ' ')
            {
                lexer.position += 1;
                continue;
            }
            if (Char.IsDigit(lexer.text[lexer.position]))
            {
                tokens.Add(TokenRepository.MakeNumber(Logger));
                continue;
            }
            if (Char.IsLetter(lexer.text[lexer.position]))
            {
                tokens.Add(TokenRepository.MakeIdentifier(Logger));
                continue;
            }
            switch (lexer.text[lexer.position])
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
                    lexer.position += 1;
                    continue;
                case '*':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.MULTIPLY, Logger));
                    lexer.position += 1;
                    continue;
                case '/':
                    tokens.Add(TokenRepository.MakeDivide(Logger));
                    continue;
                case '^':
                    tokens.Add(new Token(TokenGroup.OPERATORS, TokenOperators.POWER, Logger));
                    lexer.position += 1;
                    continue;
                case '(':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTPARENTHESES, Logger));
                    lexer.position += 1;
                    continue;
                case ')':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTPARENTHESES, Logger));
                    lexer.position += 1;
                    continue;
                case '{':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTCURLYBRACE, Logger));
                    lexer.position += 1;
                    continue;
                case '}':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTCURLYBRACE, Logger));
                    lexer.position += 1;
                    continue;
                case ',':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COMMA, Logger));
                    lexer.position += 1;
                    continue;
                case ':':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.COLON, Logger));
                    lexer.position += 1;
                    continue;
                case ';':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.SEMICOLON, Logger));
                    lexer.position += 1;
                    continue;
                case '.':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOT, Logger));
                    lexer.position += 1;
                    continue;
                case '$':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.DOLLAR, Logger));
                    lexer.position += 1;
                    continue;
                case '[':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.LEFTBRACKET, Logger));
                    lexer.position += 1;
                    continue;
                case ']':
                    tokens.Add(new Token(TokenGroup.SYNTAX, TokenSyntax.RIGHTBRACKET, Logger));
                    lexer.position += 1;
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
