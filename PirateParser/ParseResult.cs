using PirateParser.Node.Interfaces;

namespace PirateParser;

/// <summary>
/// A object which is returned by the parser.
/// </summary>
public class ParseResult
{
    public INode Node { get; set; }
    public int Index { get; set; }

    public ParseResult(INode node, int index)
    {
        Node = node;
        Index = index;
    }
}