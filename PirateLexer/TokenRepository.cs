using System.Globalization;
using PirateLexer.Tokens;
using PirateLexer.Enums;
using PirateLexer.Interfaces;
using PirateLexer.Tokens.Interfaces;

namespace PirateLexer;

/// <summary>
/// A class which handles the complex logic of the lexer.
/// </summary>
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
            token = new Token(TokenGroup.VALUE, TokenType.INT, int.Parse(numberString));

        }
        else
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = "."; cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            token = new Token(TokenGroup.VALUE, TokenType.FLOAT, float.Parse(numberString, cultureInfo));
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }

    public TokenResult MakeIdentifier(string text, int position)
    {
        var idString = string.Empty;

        while (Char.IsLetter(text[position]) || !Char.IsNumber(text[position]) || !Char.IsWhiteSpace(text[position]) || !Char.IsSeparator(text[position]))
        {
            idString += text[position];
            position += 1;
            if (position == text.Length) break;
            if (text[position] == '.')
            {
                idString += text[position];
                position += 1;
            }
            if (Char.IsNumber(text[position]) || Char.IsPunctuation(text[position]) || Char.IsWhiteSpace(text[position]) || Char.IsSeparator(text[position])) break;
        }

        var TokenTypeType = _KeyWordService.GetTypeKeyword(idString);
        if (TokenTypeType != TokenType.Empty)
        {
            return new TokenResult()
            {
                Token = new Token(TokenGroup.TYPEKEYWORD, TokenTypeType, idString),
                Position = position
            };
        }

        TokenTypeType = _KeyWordService.GetTokenControlKeyword(idString);

        if (TokenTypeType != TokenType.Empty)
        {
            return new TokenResult()
            {
                Token = new Token(TokenGroup.CONTROLKEYWORD, TokenTypeType, idString),
                Position = position
            };
        }

        return new TokenResult()
        {
            Token = new Token(TokenGroup.SYNTAX, TokenType.IDENTIFIER, idString),
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
            Token = new Token(TokenGroup.VALUE, TokenType.STRING, resultString),
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
            Token = new Token(TokenGroup.VALUE, TokenType.CHAR, resultString),
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
                Token = new Token(TokenGroup.COMPARISONOPERATORS, TokenType.NOTEQUALS),
                Position = position
            };
        }

        position += 1;
        throw new Exception("Expected Character Error: '=' (after '!')");
    }

    public TokenResult MakeGreaterThan(string text, int position)
    {
        var tokenType = TokenType.GREATERTHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenType.GREATERTHANEQUALS;
        }

        return new TokenResult()
        {
            Token = new Token(TokenGroup.COMPARISONOPERATORS, tokenType),
            Position = position
        };

    }

    public TokenResult MakeLessThan(string text, int position)
    {
        var tokenType = TokenType.LESSTHAN;
        position += 1;

        if (text[position] == '=')
        {
            position += 1;
            tokenType = TokenType.LESSTHANEQUALS;
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
            token = new Token(TokenGroup.COMPARISONOPERATORS, TokenType.DOUBLEEQUALS);
        }
        else
        {
            token = new Token(TokenGroup.SYNTAX, TokenType.EQUALS);
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
            token = new Token(TokenGroup.SYNTAX, TokenType.PLUSEQUALS);
        }
        else
        {
            token = new Token(TokenGroup.OPERATORS, TokenType.PLUS);
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
            token = new Token(TokenGroup.SYNTAX, TokenType.DOUBLEDIVIDE);
        }
        else
        {
            token = new Token(TokenGroup.OPERATORS, TokenType.DIVIDE);
        }

        return new TokenResult()
        {
            Token = token,
            Position = position
        };
    }
}
