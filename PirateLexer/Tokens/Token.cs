using Pirate.Lexer.Enums;

namespace Pirate.Lexer.Tokens;

/// <summary>
/// A model for all the tokens.
/// </summary>
public class Token
{
    public TokenGroup TokenGroup { get; set; }
    public TokenType TokenType { get; set; }
    public object Value { get; set; }

    public Token(TokenGroup tokenGroup, TokenType tokenType, object value = null)
    {
        TokenGroup = tokenGroup;
        TokenType = tokenType;
        Value = value;
    }

    public bool Matches(object tokenType, object value = null)
    {
        if (value == null || Value == null)
        {
            return TokenType.Equals(tokenType);
        }
        return TokenType.Equals(tokenType) && Value.Equals(value); ;
    }

    public override string ToString()
    {
        return $"{TokenGroup.ToString()}:{TokenType.ToString()}:{(Value != null ? Value.ToString() : "None")}";
    }
}