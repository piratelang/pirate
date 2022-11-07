using System.Globalization;
using PirateLexer.Tokens;
using PirateLexer.Enums;
using PirateLexer.Interfaces;

namespace PirateLexer;

public class TokenRepository : ITokenRepository
{
    private readonly IKeyWordService _KeyWordService;

    public TokenRepository(IKeyWordService KeyWordService)
    {
        _KeyWordService = KeyWordService;
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

        }
        else
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = "."; cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            token = new Token(TokenGroup.VALUE, TokenValue.FLOAT, float.Parse(numberString, cultureInfo));
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

        var tokenTypeKeywordType = _KeyWordService.GetTypeKeyword(idString);
        if (tokenTypeKeywordType != TokenTypeKeyword.Empty)
        {
            token = new Token(TokenGroup.TYPEKEYWORD, tokenTypeKeywordType, idString);
        }

        var tokenControlKeywordType = _KeyWordService.GetTokenControlKeywork(idString);

        if (tokenControlKeywordType != TokenControlKeyword.Empty)
        {
            token = new Token(TokenGroup.CONTROLKEYWORD, tokenControlKeywordType, idString);
        }

        else
        {
            token = new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, idString);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakeString(string text, int position)
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

        return new TokenResult()
        {
            Token = new Token(TokenGroup.VALUE, TokenValue.STRING, resultString),
            Position = position
        };
    }

    public TokenResult MakeChar(string text, int position)
    {
        position += 1;
        var resultString = text[position];
        position += 1;
        if (text[position] != '\'')
        {
            throw new InvalidOperationException("Char is not one letter");
        }
        position += 1;

        return new TokenResult()
        {
            Token = new Token(TokenGroup.VALUE, TokenValue.CHAR, resultString),
            Position = position
        };
    }

    public TokenResult MakeNotEquals(string text, int position)
    {
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            return new TokenResult()
            {
                Token = new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.NOTEQUALS),
                Position = position
            };
        }

        position += 1;
        throw new Exception("Expected Character Error: '=' (after '!')");
    }

    public TokenResult MakeGreaterThan(string text, int position)
    {
        var tokenType = TokenComparisonOperators.GREATERHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenComparisonOperators.GREATERTHANEQUALS;
        }

        return new TokenResult()
        {
            Token = new Token(TokenGroup.COMPARISONOPERATORS, tokenType),
            Position = position
        };

    }

    public TokenResult MakeLessThan(string text, int position)
    {
        var tokenType = TokenComparisonOperators.LESSTHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenComparisonOperators.LESSTHANEQUALS;
        }

        return new TokenResult()
        {
            Token = new Token(TokenGroup.COMPARISONOPERATORS, tokenType),
            Position = position
        };
    }

    public TokenResult MakeEquals(string text, int position)
    {
        Token token;
        position += 1;
        if (text[position] == '=')
        {
            position += 1;
            token = new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS);
        }
        else
        {
            token = new Token(TokenGroup.SYNTAX, TokenSyntax.EQUALS);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakePlus(string text, int position)
    {
        Token token;
        position += 1;
        if (text[position] == '=')
        {
            position += 1;
            token = new Token(TokenGroup.SYNTAX, TokenSyntax.PLUSEQUALS);
        }
        else
        {
            token = new Token(TokenGroup.OPERATORS, TokenOperators.PLUS);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakeDivide(string text, int position)
    {
        Token token;
        position += 1;
        if (text[position] == '/')
        {
            position += 1;
            token = new Token(TokenGroup.SYNTAX, TokenSyntax.DOUBLEDIVIDE);
        }
        else
        {
            token = new Token(TokenGroup.OPERATORS, TokenOperators.DIVIDE);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }
}
