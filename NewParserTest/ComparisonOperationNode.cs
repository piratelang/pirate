namespace NewParserTest;

public class ComparisonOperationNode: INode
{
    public Token Left { get; set; }
    public Token Operator { get; set; }
    public Token Right { get; set; }
}