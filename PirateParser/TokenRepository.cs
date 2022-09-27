using PirateParser.Models;

namespace PirateParser
{
    public class TokenRepository
    {
        public static Token MakeNumber()
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
                return new Token(TokenType.INT, int.Parse(numberString), positionStart, Lexer.position);
            }
            else
            {
                return new Token(TokenType.FLOAT, float.Parse(numberString), positionStart, Lexer.position);
            }
        }

        public static Token MakeIdentifier()
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
            else
            {
                tokenType = TokenType.IDENTIFIER;
            }

            return new Token(tokenType, idString, positionStart, Lexer.position);
        }

        public static Token MakeString()
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
            return new Token(TokenType.STRING, resultString, positionStart, Lexer.position);

        }

        public static (Token token, Error error) MakeNotEquals()
        {
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                return (new Token(TokenType.NOTEQUALS, PositionStart: positionStart, PositionEnd: Lexer.position), null);
            }

            Lexer.Advance();
            return (null, new Error(positionStart, Lexer.position, "Expected Character Error", "'=' (after '!')"));
        }

        public static Token MakeGreaterThan()
        {
            var tokenType = TokenType.GREATHERTHAN;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.GREATERTHANEQUALS;
            }

            return new Token(tokenType, PositionStart: positionStart, PositionEnd: Lexer.position);
        }

        public static Token MakeLessThan()
        {
            var tokenType = TokenType.LESSTHAN;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.LESSTHANEQUALS;
            }

            return new Token(tokenType, PositionStart: positionStart, PositionEnd: Lexer.position);
        }

        public static Token MakeEquals()
        {
            var tokenType = TokenType.EQUALS;
            var positionStart = Lexer.position.Copy();
            Lexer.Advance();

            if (Lexer.currentChar == '=')
            {
                Lexer.Advance();
                tokenType = TokenType.DOUBLEEQUALS;
            }

            return new Token(tokenType, PositionStart: positionStart, PositionEnd: Lexer.position);
        }
    }
}