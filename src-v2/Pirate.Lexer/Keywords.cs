namespace Pirate.Lexer;

/// <summary>
/// Reserved words per docs/GRAMMAR.md §1.3. An identifier matching one of
/// these lexes as the keyword, not as TokenType.Identifier.
/// </summary>
internal static class Keywords
{
    private static readonly Dictionary<string, TokenType> Map = new(StringComparer.Ordinal)
    {
        ["var"] = TokenType.Var,
        ["int"] = TokenType.Int,
        ["float"] = TokenType.Float,
        ["string"] = TokenType.String,
        ["char"] = TokenType.Char,
        ["bool"] = TokenType.Bool,
        ["void"] = TokenType.Void,
        ["func"] = TokenType.Func,
        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["while"] = TokenType.While,
        ["for"] = TokenType.For,
        ["in"] = TokenType.In,
        ["to"] = TokenType.To,
        ["return"] = TokenType.Return,
        ["extern"] = TokenType.Extern,
        ["true"] = TokenType.True,
        ["false"] = TokenType.False,
        ["class"] = TokenType.Class,
        ["new"] = TokenType.New,
    };

    public static bool TryGetKeyword(string identifier, out TokenType type) =>
        Map.TryGetValue(identifier, out type);
}
