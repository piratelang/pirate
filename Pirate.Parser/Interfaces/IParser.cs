namespace Pirate.Parser.Interfaces;

public interface IParser
{
    ILogger Logger { get; set; }

    Scope StartParse(List<Token> tokens, string fileName);
}
