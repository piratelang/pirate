using System.Net;
using Interpreter.Interpreters.Interfaces;
using Interpreter.Values;
using Lexer.Enums;
using Parser.Node;
using Parser.Node.Interfaces;
using Lexer.Tokens;

namespace Interpreter.Interpreters;

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