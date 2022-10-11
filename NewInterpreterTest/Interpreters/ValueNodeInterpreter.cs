using System.Net;
using NewInterpreterTest.Interpreters.Interfaces;
using NewInterpreterTest.Values;
using NewLexerTest.Enums;
using NewParserTest.Node;
using NewParserTest.Node.Interfaces;
using NewPirateLexer.Tokens;

namespace NewInterpreterTest.Interpreters;

public class ValueNodeInterpreter : BaseInterpreter
{
    private IValueNode Node { get; set; }
    public ValueNodeInterpreter(INode node)
    {
        Node = node as IValueNode;
    }

    public override BaseValue VisitNode()
    {
        return new Number(Node.Value.Value);
    }
}