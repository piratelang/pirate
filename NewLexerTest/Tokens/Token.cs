using NewPirateLexer.Enums;

namespace NewPirateLexer.Tokens;
public class Token
{
    public TokenGroup tokenGroup { get; set; }
    public object tokenType { get; set; }
    public object? value { get; set; }

    public Token(TokenGroup TokenGroup, object TokenType, object Value = null)
    {
        tokenGroup = TokenGroup;
        tokenType = TokenType;
        value = Value;
    }

    public bool Matches(object TokenType, object Value)
    {
        return tokenType == TokenType && value == Value;
    }

    public string Display()
    {
        if (value != null)
        {
            return $"{tokenGroup.ToString()}:{tokenType.ToString()}:{value.ToString()}";
        }
        return $"{tokenGroup.ToString()}:{tokenType.ToString()}";
    }
}