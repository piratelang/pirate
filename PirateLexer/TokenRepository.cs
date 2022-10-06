using Common;
using PirateLexer.Models;

namespace PirateLexer
{
    public class TokenRepository
    {
        public static Token MakeNumber(Logger logger)
        {
            var numberString = string.Empty; 
            var dotCount = 0;
            var positionStart = Lexer.position.Copy();

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
                return new Token(TokenType.INT, logger, int.Parse(numberString), positionStart, Lexer.position);
            }
            else
            {
                return new Token(TokenType.FLOAT, logger, float.Parse(numberString), positionStart, Lexer.position);
            }
        }

        public static Token MakeIdentifier(Logger logger)
        {
            var idString = string.Empty;
            var positionStart = Lexer.position.Copy();

            while (Lexer.currentChar != null && (Globals.LETTERS_DIGITS.Contains(Lexer.currentChar) || Lexer.currentChar == '_'))
            {
                idString += Lexer.currentChar;
                Lexer.Advance();
            }

            var tokenType = new TokenType();

            if (Globals.KEYWORDS.Contains(idString))
            {
                tokenType = TokenType.KEYWORD;
            }
            else if (Globals.BOOLEANS.Contains(idString))
            {
                tokenType = TokenType.BOOLEAN;
            }
            else
            {
                tokenType = TokenType.IDENTIFIER;
            }

            return new Token(tokenType, logger, idString, positionStart, Lexer.position);
        }

        public static Token MakeString(Logger logger)
        {
            var resultString = string.Empty;
            var positionStart = Lexer.position.Copy();
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
            return new Token(TokenType.STRING, logger, resultString, positionStart, Lexer.position);

        }

        public static (Token token, Error error) MakeNotEquals(Logger logger)
        {
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                return (new Token(TokenType.NOTEQUALS, logger, PositionStart: positionStart, PositionEnd: Lexer.position), null);
            }

            Lexer.Advance();
            return (null, new Error(positionStart, Lexer.position, "Expected Character Error", "'=' (after '!')"));
        }

        public static Token MakeGreaterThan(Logger logger)
        {
            var tokenType = TokenType.GREATERHAN;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.GREATERTHANEQUALS;
            }

            return new Token(tokenType, logger, PositionStart: positionStart, PositionEnd: Lexer.position);
        }

        public static Token MakeLessThan(Logger logger)
        {
            var tokenType = TokenType.LESSTHAN;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.LESSTHANEQUALS;
            }

            return new Token(tokenType, logger, PositionStart: positionStart, PositionEnd: Lexer.position);
        }

        public static Token MakeEquals(Logger logger)
        {
            var tokenType = TokenType.EQUALS;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.DOUBLEEQUALS;
            }

            return new Token(tokenType, logger, PositionStart: positionStart, PositionEnd: Lexer.position);
        }

        public static Token MakePlus(Logger logger)
        {
            var tokenType = TokenType.PLUS;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.PLUSEQUALS;
            }

            return new Token(tokenType, logger, PositionStart: positionStart, PositionEnd: Lexer.position);
        }
        public static Token MakeDivide(Logger logger)
        {
            var tokenType = TokenType.DIVIDE;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '/')
            {
                Lexer.Advance();
                tokenType = TokenType.DOUBLEDIVIDE;
            }

            return new Token(tokenType, logger, PositionStart: positionStart, PositionEnd: Lexer.position);
        }
    }
}