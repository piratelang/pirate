using Lexer.Tokens;
using Common.Interfaces;

namespace Parser;

public interface IParser
{
    ILogger Logger { get; set; }
    IObjectSerializer ObjectSerializer { get; set; }

    Scope StartParse(List<Token> tokens, string fileName);
}
