using PirateLexer.Enums;

namespace PirateLexer.Tokens;

public class KeyWorkService : IKeyWorkService
{
    private string[] typeKeywords = new string[] { "var", "int", "float", "string", "char", "new" };

    private string[] controlKeywords = new string[] { "if", "else", "for", "to", "foreach", "in", "while", "func", "class" };

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
            }
            throw new TyepKeyWorkNotImplementedException();
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
            throw new ControlKeyWorkNotImplementedException();
        }
        return TokenControlKeyword.Empty;
    }
}
