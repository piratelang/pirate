using PirateLexer.Tokens;

namespace PirateLexer.Interfaces;

/// <inheritdoc cref="Lexer"/>
public interface ILexer
{
    ILogger Logger { get; set; }
    string fileName { get; set; }
    string text { get; set; }
    int position { get; set; }

    List<Token> MakeTokens(string Text, string FileName);
}
