using Common;
using Lexer.Enums;

namespace Lexer.Tokens;

[Serializable]
public class Token
{
    public TokenGroup TokenGroup { get; set; }
    public object TokenType { get; set; }
    public object? Value { get; set; }

    public Token(TokenGroup tokenGroup, object tokenType, ILogger logger, object value = null)
    {
        TokenGroup = tokenGroup;
        TokenType = tokenType;
        Value = value;
        logger.Log($"Creating Token \"{ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public bool Matches(object tokenType, object Value=null)
    {
        if (Value == null)
        {
            return TokenType.Equals(tokenType);
        }
        return this.TokenType == TokenType && this.Value == Value;
    }

    public override string ToString()
    {
        return $"{TokenGroup.ToString()}:{TokenType.ToString()}:{(Value != null ? Value.ToString() : "None")}";
    }
}