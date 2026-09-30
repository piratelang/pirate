namespace Pirate.Lexer.TokenType;

public class TokenTypeMapper
{
    public static Pirate.Lexer.TokenType.Enums.TokenType ConvertTokenType(Pirate.Lexer.Enums.TokenType tokenType)
    {
        return Enum.Parse<Pirate.Lexer.TokenType.Enums.TokenType>(tokenType.ToString());
    }

    public static Pirate.Lexer.TokenType.Enums.TokenGroup ConvertTokenGroup(Pirate.Lexer.Enums.TokenGroup tokenGroup)
    {
        return Enum.Parse<Pirate.Lexer.TokenType.Enums.TokenGroup>(tokenGroup.ToString());
    }
}

