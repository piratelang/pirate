namespace PirateLexer.Models
{
    public class Token
    {
        public TokenType tokenType { get; set; }
        public object? value { get; set; }

        public Position? positionStart { get; set; }
        public Position? positionEnd { get; set; }

        public Token(TokenType TokenType, object Value = null, Position PositionStart = null, Position PositionEnd = null)
        {
            tokenType = TokenType;
            value = Value;

            if (PositionStart != null)
            {
                positionStart = PositionStart;
                positionEnd = PositionStart;
            }
            if (PositionEnd != null)
            {
                positionEnd = PositionEnd;
            }
        }

        public bool Matches(TokenType Type, object Value)
        {
            return tokenType == Type && value == Value;
        }

        public string Display()
        {
            if (value != null)
            {
                return $"{tokenType.ToString()}:{value.ToString()}";
            }
            return $"{tokenType.ToString()}";
        }
    }
}