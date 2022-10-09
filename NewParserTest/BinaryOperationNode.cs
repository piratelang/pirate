using NewPirateLexer.Tokens;

namespace NewParserTest;

public class BinaryOperationNode: INode
{
    public Token Left { get; set; }
    public Token Operator { get; set; }
    public Token Right { get; set; }
}
