using Common;
using Common.Enum;
using Lexer.Enums;

namespace Lexer.Tokens;

public class Token
{
    public TokenGroup TokenGroup { get; set; }
    public object TokenType { get; set; }
    public object? Value { get; set; }

    public Token(TokenGroup tokenGroup, object tokenType, Logger logger, object value = null)
    {
        TokenGroup = tokenGroup;
        TokenType = tokenType;
        Value = value;
        logger.Log($"Creating Token \"{TokenGroup.ToString()} | {TokenType.ToString()} | {(Value != null ? Value.ToString() : "None")}\"", this.GetType().Name, LogType.INFO);
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