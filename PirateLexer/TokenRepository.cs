using System.Globalization;
using PirateLexer.Tokens;
using PirateLexer.Enums;

namespace Pirate

public interface ITokenRepository
{
    TokenResult MakeChar(ILogger Logger);
    TokenResult MakeDivide(ILogger Logger);
    TokenResult MakeEquals(ILogger Logger);
    TokenResult MakeGreaterThan(ILogger Logger);
    TokenResult MakeIdentifier(ILogger Logger);
    TokenResult MakeLessThan(ILogger Logger);
    TokenResult MakeNotEquals(ILogger Logger);
    TokenResult MakeNumber(ILogger Logger, char currentChar);
    TokenResult MakePlus(ILogger Logger);
    TokenResult MakeString(ILogger Logger);
}

public class TokenRepository : ITokenRepository
{
    private readonly ILogger _logger;
    private readonly IKeyWorkService _keyWorkService;

    public TokenRepository(ILogger logger, IKeyWorkService keyWorkService)
    {
        _logger = logger;
        _keyWorkService = keyWorkService;
    }

    public TokenResult MakeNumber(string text, int position)
    {
        var dotCount = 0;
        var numberString = string.Empty;
        Token token;

        while (Char.IsDigit(text[position]) || text[position] == '.')
        {
            if (text[position] == '.')
            {
                if (dotCount == 1)
                {
                    break;
                }
                dotCount += 1;
                numberString += '.';
            }
            else
            {
                numberString += text[position];
            }
            position += 1;
            if (position == text.Length) break;
        }

        if (dotCount == 0)
        {
            token = new Token(TokenGroup.VALUE, TokenValue.INT, int.Parse(numberString));
            _logger.Log($"Creating Token \"{token.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);

        }
        else
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = "."; cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            token = new Token(TokenGroup.VALUE, TokenValue.FLOAT, float.Parse(numberString, cultureInfo));
            _logger.Log($"Creating Token \"{token.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakeIdentifier(string text, int position)
    {
        Token token;
        var idString = string.Empty;

        while (Char.IsLetterOrDigit(text[position]) || text[position] != ' ')
        {
            idString += text[position];
            position += 1;
            if (position == text.Length) break;
        }

        var tokenTypeKeywordType = _keyWorkService.GetTypeKeyword(idString);
        if ( tokenTypeKeywordType != TokenTypeKeyword.Empty)
        {
            token = new Token(TokenGroup.TYPEKEYWORD, tokenTypeKeywordType, idString);
        }

        var tokenControlKeywordType = _keyWorkService.GetTokenControlKeywork(idString);

        if (tokenControlKeywordType != TokenControlKeyword.Empty)
        {
            token = new Token(TokenGroup.CONTROLKEYWORD, tokenControlKeywordType, idString);
        }

        else
        {
            token = new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER,  idString);
        }
        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakeString()
    {
  

        var resultString = string.Empty;
        var escapeCharacter = false;
        position += 1;

        Dictionary<string, string> escapeCharacters = new Dictionary<string, string>() { };
        escapeCharacters.Add("n", "\n");
        escapeCharacters.Add("t", "\t");

        while (text[position] != '"' || escapeCharacter)
        {
            if (escapeCharacter)
            {
                resultString += escapeCharacters[text[position].ToString()];
            }
            else
            {
                if (text[position] == '\\')
                {
                    escapeCharacter = true;
                }
                else
                {
                    resultString += text[position];
                }
            }
            position += 1;
            escapeCharacter = false;
            if (position == text.Length) break;
        }
        position += 1;
        return new Token(TokenGroup.VALUE, TokenValue.STRING, Logger, resultString);
    }

    public TokenResult MakeChar()
    {


        position += 1;
        var resultString = text[position];
        position += 1;
        if (text[position] != '\'')
        {
            throw new InvalidOperationException("Char is not one letter");
        }
        position += 1;
        return new Token(TokenGroup.VALUE, TokenValue.CHAR, Logger, resultString);
    }

    public TokenResult MakeNotEquals()
    {


        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            return new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.NOTEQUALS, Logger);
        }

        position += 1;
        throw new Exception("Expected Character Error: '=' (after '!')");
    }

    public TokenResult MakeGreaterThan()
    {

        var tokenType = TokenComparisonOperators.GREATERHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenComparisonOperators.GREATERTHANEQUALS;
        }

        return new Token(TokenGroup.COMPARISONOPERATORS, tokenType, Logger);
    }

    public TokenResult MakeLessThan()
    {
        var = Instance(Logger);

        var tokenType = TokenComparisonOperators.LESSTHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenComparisonOperators.LESSTHANEQUALS;
        }

        return new Token(TokenGroup.COMPARISONOPERATORS, tokenType, Logger);
    }

    public TokenResult MakeEquals()
    {
        var = Instance(Logger);

        position += 1;
        if (text[position] == '=')
        {
            position += 1;
            return new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, Logger);
        }
        else
        {
            return new Token(TokenGroup.SYNTAX, TokenSyntax.EQUALS, Logger);
        }
    }

    public TokenResult MakePlus()
    {
        var = Instance(Logger);

        position += 1;
        if (text[position] == '=')
        {
            position += 1;
            return new Token(TokenGroup.SYNTAX, TokenSyntax.PLUSEQUALS, Logger);
        }
        else
        {
            return new Token(TokenGroup.OPERATORS, TokenOperators.PLUS, Logger);
        }
    }

    public TokenResult MakeDivide()
    {
        var = Instance(Logger);

        position += 1;
        if (text[position] == '/')
        {
            position += 1;
            return new Token(TokenGroup.SYNTAX, TokenSyntax.DOUBLEDIVIDE, Logger);
        }
        else
        {
            return new Token(TokenGroup.OPERATORS, TokenOperators.DIVIDE, Logger);
        }
    }

    {
        throw new NotImplementedException();
    }
}
