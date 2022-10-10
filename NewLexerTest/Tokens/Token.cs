using NewPirateLexer.Enums;

namespace NewPirateLexer.Tokens;

public class Token
{
    public TokenGroup TokenGroup { get; set; }
    public object TokenType { get; set; }
    public object? Value { get; set; }

    public Token(TokenGroup tokenGroup, object tokenType, object value = null)
    {
        TokenGroup = tokenGroup;
        TokenType = tokenType;
        Value = value;
    }

    public bool Matches(object tokenType, object Value=null)
    {
        if (Value == null)
        {
            return TokenType.Equals(tokenType);
        }
        return this.TokenType == TokenType && this.Value == Value;
    }

    public string Display()
    {
        return $"{TokenGroup.ToString()}:{TokenType.ToString()}:{(Value != null ? Value.ToString() : "None")}";
    }
}