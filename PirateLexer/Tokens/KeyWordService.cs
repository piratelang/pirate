using PirateLexer.Enums;
using PirateLexer.Interfaces;

namespace PirateLexer.Tokens;

public class KeyWordService : IKeyWordService
{
    private string[] typeKeywords = new string[] { "var", "int", "float", "string", "char", "void" };

    private string[] controlKeywords = new string[] { "if", "else", "for", "to", "foreach", "in", "while", "func", "class", "new"};

    public TokenTypeKeyword GetTypeKeyword(string idString)
    {
        if (typeKeywords.Contains(idString))
        {
            switch (idString)
            {
                case "var":
                    return TokenTypeKeyword.VAR;
                case "int":
                    return TokenTypeKeyword.INT;
                case "float":
                    return TokenTypeKeyword.FLOAT;
                case "string":
                    return TokenTypeKeyword.STRING;
                case "char":
                    return TokenTypeKeyword.CHAR;
                case "void":
                    return TokenTypeKeyword.VOID;
            }
            throw new NotImplementedException($"Type keyword, {idString} has not been implemented");
        }
        return TokenTypeKeyword.Empty;
    }

    public TokenControlKeyword GetTokenControlKeywork(string idString)
    {
        if (controlKeywords.Contains(idString))
        {
            switch (idString)
            {
                case "if":
                    return TokenControlKeyword.IF;

                case "else":
                    return TokenControlKeyword.ELSE;

                case "for":
                    return TokenControlKeyword.FOR;

                case "to":
                    return TokenControlKeyword.TO;

                case "foreach":
                    return TokenControlKeyword.FOREACH;

                case "in":
                    return TokenControlKeyword.IN;

                case "while":
                    return TokenControlKeyword.WHILE;

                case "func":
                    return TokenControlKeyword.FUNC;

                case "class":
                    return TokenControlKeyword.CLASS;

                case "new":
                    return TokenControlKeyword.NEW;
            }
            throw new NotImplementedException($"Control keyword, {idString} has not been implemented");
        }
        return TokenControlKeyword.Empty;
    }
}
