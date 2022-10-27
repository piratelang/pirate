using System.Globalization;
using Lexer.Enums;
using Lexer.Tokens;

namespace Lexer;

public class TokenRepository
{
    public static Token MakeNumber(ILogger Logger)
    {
        var numberString = string.Empty;
        var dotCount = 0;

        while (Lexer.Instance(Logger).text[Lexer.Instance(Logger).position] != null && (Globals.DIGITS.Contains(Lexer.currentChar) || Lexer.currentChar == '.'))
        {
            if (Lexer.currentChar == '.')
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
                numberString += Lexer.currentChar;
            }
            Lexer.Advance();
        }

        if (dotCount == 0)
        {
            return new Token(TokenGroup.VALUE, TokenValue.INT, Logger, int.Parse(numberString));
        }
        else
        {
            CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = "."; cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            return new Token(TokenGroup.VALUE, TokenValue.FLOAT, Logger, float.Parse(numberString, cultureInfo));
        }
    }

    public static Token MakeIdentifier(ILogger Logger)
    {
        var idString = string.Empty;
        var tokenType = new object();
        var typeKeywords = new string[] { "var", "int", "float", "string", "char", "new" };
        var controlKeywords = new string[] { "if", "else", "for", "to", "foreach", "in", "while", "func", "class" };

        while (Lexer.currentChar != null && (Globals.LETTERS_DIGITS.Contains(Lexer.currentChar) || Lexer.currentChar == '_'))
        {
            idString += Lexer.currentChar;
            Lexer.Advance();
        }

        if (typeKeywords.Contains(idString))
        {
            var tokenGroup = TokenGroup.TYPEKEYWORD;
            switch (idString)
            {
                case "var":
                    tokenType = TokenTypeKeyword.VAR;
                    break;
                case "int":
                    tokenType = TokenTypeKeyword.INT;
                    break;
                case "float":
                    tokenType = TokenTypeKeyword.FLOAT;
                    break;
                case "string":
                    tokenType = TokenTypeKeyword.STRING;
                    break;
                case "char":
                    tokenType = TokenTypeKeyword.CHAR;
                    break;
            }
            return new Token(tokenGroup, tokenType, Logger, idString);
        }
        else if (controlKeywords.Contains(idString))
        {
            var tokenGroup = TokenGroup.CONTROLKEYWORD;
            switch (idString)
            {
                case "if":
                    tokenType = TokenControlKeyword.IF;
                    break;
                case "else":
                    tokenType = TokenControlKeyword.ELSE;
                    break;
                case "for":
                    tokenType = TokenControlKeyword.FOR;
                    break;
                case "to":
                    tokenType = TokenControlKeyword.TO;
                    break;
                case "foreach":
                    tokenType = TokenControlKeyword.FOREACH;
                    break;
                case "in":
                    tokenType = TokenControlKeyword.IN;
                    break;
                case "while":
                    tokenType = TokenControlKeyword.WHILE;
                    break;
                case "func":
                    tokenType = TokenControlKeyword.FUNC;
                    break;
                case "class":
                    tokenType = TokenControlKeyword.CLASS;
                    break;
                case "new":
                    tokenType = TokenControlKeyword.NEW;
                    break;
            }
            return new Token(tokenGroup, tokenType, Logger, idString);
        }
        else
        {
            return new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, Logger, idString);
        }
    }

    public static Token MakeString(ILogger Logger)
    {
        var resultString = string.Empty;
        var escapeCharacter = false;
        Lexer.Advance();

        Dictionary<string, string> escapeCharacters = new Dictionary<string, string>() { };
        escapeCharacters.Add("n", "\n");
        escapeCharacters.Add("t", "\t");

        while (Lexer.currentChar != null && Lexer.currentChar != '"' || escapeCharacter)
        {
            if (escapeCharacter)
            {
                resultString += escapeCharacters[Lexer.currentChar.ToString()];
            }
            else
            {
                if (Lexer.currentChar == '\\')
                {
                    escapeCharacter = true;
                }
                else
                {
                    resultString += Lexer.currentChar;
                }
            }
            Lexer.Advance();
            escapeCharacter = false;
        }
        Lexer.Advance();
        return new Token(TokenGroup.VALUE, TokenValue.STRING, Logger, resultString);
    }

    public static Token MakeChar(ILogger Logger)
    {
        Lexer.Advance();
        var resultString = Lexer.currentChar;
        Lexer.Advance();
        if (Lexer.currentChar != '\'')
        {
            throw new NotImplementedException("Char is not one letter");
        }
        Lexer.Advance();
        return new Token(TokenGroup.VALUE, TokenValue.CHAR, Logger, resultString);
    }

    public static (Token token, Error error) MakeNotEquals(ILogger Logger)
    {
        Lexer.Advance();

        if (Lexer.currentChar == '=')
        {
            Lexer.Advance();
            return (new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.NOTEQUALS, Logger), null);
        }

        Lexer.Advance();
        return (null, new Error("Expected Character Error", "'=' (after '!')"));
    }

    public static Token MakeGreaterThan(ILogger Logger)
    {
        var tokenType = TokenComparisonOperators.GREATERHAN;
        Lexer.Advance();

        if (Lexer.currentChar == '=')
        {
            Lexer.Advance();
            tokenType = TokenComparisonOperators.GREATERTHANEQUALS;
        }

        return new Token(TokenGroup.COMPARISONOPERATORS, tokenType, Logger);
    }

    public static Token MakeLessThan(ILogger Logger)
    {
        var tokenType = TokenComparisonOperators.LESSTHAN;
        Lexer.Advance();

        if (Lexer.currentChar == '=')
        {
            Lexer.Advance();
            tokenType = TokenComparisonOperators.LESSTHANEQUALS;
        }

        return new Token(TokenGroup.COMPARISONOPERATORS, tokenType, Logger);
    }

    public static Token MakeEquals(ILogger Logger)
    {
        Lexer.Advance();
        if (Lexer.currentChar == '=')
        {
            Lexer.Advance();
            return new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS, Logger);
        }
        else
        {
            return new Token(TokenGroup.SYNTAX, TokenSyntax.EQUALS, Logger);
        }
    }

    public static Token MakePlus(ILogger Logger)
    {
        Lexer.Advance();
        if (Lexer.currentChar == '=')
        {
            Lexer.Advance();
            return new Token(TokenGroup.SYNTAX, TokenSyntax.PLUSEQUALS, Logger);
        }
        else
        {
            return new Token(TokenGroup.OPERATORS, TokenOperators.PLUS, Logger);
        }
    }
    public static Token MakeDivide(ILogger Logger)
    {
        Lexer.Advance();
        if (Lexer.currentChar == '/')
        {
            Lexer.Advance();
            return new Token(TokenGroup.SYNTAX, TokenSyntax.DOUBLEDIVIDE, Logger);
        }
        else
        {
            return new Token(TokenGroup.OPERATORS, TokenOperators.DIVIDE, Logger);
        }
    }
}
