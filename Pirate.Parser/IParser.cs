using Pirate.Lexer.Tokens;

namespace Pirate.Parser;

public interface IParser
{
    ILogger Logger { get; set; }
    IObjectSerializer ObjectSerializer { get; set; }

    Scope StartParse(List<Token> tokens, string fileName);
}
