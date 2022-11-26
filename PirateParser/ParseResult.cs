using PirateParser.Node.Interfaces;

namespace PirateParser;

public class ParseResult
{
    public INode node { get; set; }
    public int index { get; set; }

    public ParseResult(INode node, int index)
    {
        this.node = node;
        this.index = index;
    }
}