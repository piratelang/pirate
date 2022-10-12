using System.Security.Principal;
using Lexer.Enums;
using Lexer.Enums;
using Lexer.Tokens;

namespace Lexer
{
    public class TokenRepository
    {
        public static Token MakeNumber()
        {
            var numberString = string.Empty; 
            var dotCount = 0;

            while (Lexer.currentChar != null && (Globals.DIGITS.Contains(Lexer.currentChar) || Lexer.currentChar == '.'))
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
                return new Token(TokenGroup.VALUE, TokenValue.INT, int.Parse(numberString));
            }
            else
            {
                return new Token(TokenGroup.VALUE, TokenValue.FLOAT, float.Parse(numberString));
            }
        }

        public static Token MakeIdentifier()
        {
            var idString = string.Empty;

            while (Lexer.currentChar != null && (Globals.LETTERS_DIGITS.Contains(Lexer.currentChar) || Lexer.currentChar == '_'))
            {
                idString += Lexer.currentChar;
                Lexer.Advance();
            }

            var tokenType = new object();
            var typeKeywords = new string[] {"var", "int", "float", "string", "char", "new"};
            var controlKeywords = new string[] {"if", "else", "for", "to", "foreach", "in", "while", "func", "class"};

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
                    case "new":
                        tokenType = TokenTypeKeyword.NEW;
                        break;
                }
                return new Token(tokenGroup, tokenType, idString);
            }
            else if(controlKeywords.Contains(idString))
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
                }
                return new Token(tokenGroup, tokenType, idString);
            }
            else
            {
                return new Token(TokenGroup.SYNTAX, TokenSyntax.IDENTIFIER, idString);
            }
        }

        public static Token MakeString()
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
            return new Token(TokenGroup.VALUE, TokenValue.STRING, resultString);
        }

        public static Token MakeChar()
        {
            var resultString = ' ';
            var escapeCharacter = false;
            Lexer.Advance();

            Dictionary<string, string> escapeCharacters = new Dictionary<string, string>() { };
            escapeCharacters.Add("n", "\n");
            escapeCharacters.Add("t", "\t");

            while (Lexer.currentChar != null && Lexer.currentChar != '\'' || escapeCharacter)
            {
                if (escapeCharacter)
                {
                    resultString += Lexer.currentChar;
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
            return new Token(TokenGroup.VALUE, TokenValue.CHAR, resultString);
        }

        public static (Token token, Error error) MakeNotEquals()
        {
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                return (new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.NOTEQUALS), null);
            }

            Lexer.Advance();
            return (null, new Error("Expected Character Error", "'=' (after '!')"));
        }

        public static Token MakeGreaterThan()
        {
            var tokenType = TokenComparisonOperators.GREATERHAN;
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenComparisonOperators.GREATERTHANEQUALS;
            }

            return new Token(TokenGroup.COMPARISONOPERATORS, tokenType);
        }

        public static Token MakeLessThan()
        {
            var tokenType = TokenComparisonOperators.LESSTHAN;
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenComparisonOperators.LESSTHANEQUALS;
            }

            return new Token(TokenGroup.COMPARISONOPERATORS, tokenType);
        }

        public static Token MakeEquals()
        {
            Lexer.Advance();
            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                return new Token(TokenGroup.COMPARISONOPERATORS, TokenComparisonOperators.DOUBLEEQUALS);
            }
            else
            {
                return new Token(TokenGroup.SYNTAX, TokenSyntax.EQUALS);
            }
        }

        public static Token MakePlus()
        {
            Lexer.Advance();
            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                return new Token(TokenGroup.SYNTAX, TokenSyntax.PLUSEQUALS);
            }
            else
            {
                return new Token(TokenGroup.OPERATORS, TokenOperators.PLUS);
            }
        }
        public static Token MakeDivide()
        {
            Lexer.Advance();
            if (Lexer.currentChar == '/')
            {
                Lexer.Advance();
                return new Token(TokenGroup.SYNTAX, TokenSyntax.DOUBLEDIVIDE);
            }
            else
            {
                return new Token(TokenGroup.OPERATORS, TokenOperators.DIVIDE);
            }
        }
    }
}