using NewPirateLexer.Tokens;

namespace NewParserTest;

public interface INode
{
    Token Left { get; set; }
    Token Operator { get; set; }
    Token Right { get; set; }
}
