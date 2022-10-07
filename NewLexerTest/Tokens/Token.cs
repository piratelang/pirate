namespace NewPirateLexer.Tokens;
public class Token : IToken
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