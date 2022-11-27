using PirateLexer.Enums;
using PirateLexer.Tokens.Interfaces;

namespace PirateLexer.Tokens;

/// <summary>
/// A class which checks for keywords.
/// </summary>
public class KeyWordService : IKeyWordService
{
    private string[] typeKeywords = new string[] { "var", "int", "float", "string", "char", "void" };

    private string[] controlKeywords = new string[] { "if", "else", "for", "to", "foreach", "in", "while", "func", "class", "new", "return" };


    public TokenType GetTypeKeyword(string idString)
    {
        if (typeKeywords.Contains(idString))
        {
            switch (idString)
            {
                case "var":
                    return TokenType.VAR;
                case "int":
                    return TokenType.INT;
                case "float":
                    return TokenType.FLOAT;
                case "string":
                    return TokenType.STRING;
                case "char":
                    return TokenType.CHAR;
                case "void":
                    return TokenType.VOID;
            }
            throw new NotImplementedException($"Type keyword, {idString} has not been implemented");
        }
        return TokenType.Empty;
    }

    public TokenType GetTokenControlKeyword(string idString)
    {
        if (controlKeywords.Contains(idString))
        {
            switch (idString)
            {
                case "if":
                    return TokenType.IF;

                case "else":
                    return TokenType.ELSE;

                case "for":
                    return TokenType.FOR;

                case "to":
                    return TokenType.TO;

                case "foreach":
                    return TokenType.FOREACH;

                case "in":
                    return TokenType.IN;

                case "while":
                    return TokenType.WHILE;

                case "func":
                    return TokenType.FUNC;

                case "class":
                    return TokenType.CLASS;

                case "new":
                    return TokenType.NEW;

                case "return":
                    return TokenType.RETURN;
            }
            throw new NotImplementedException($"Control keyword, {idString} has not been implemented");
        }
        return TokenType.Empty;
    }
}
