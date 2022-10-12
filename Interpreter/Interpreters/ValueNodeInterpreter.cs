using System.Net;
using Interpreter.Interpreters.Interfaces;
using Interpreter.Values;
using Lexer.Enums;
using Parser.Node;
using Parser.Node.Interfaces;
using Lexer.Tokens;
using Common;

namespace Interpreter.Interpreters;

public class ValueNodeInterpreter : BaseInterpreter
{
    private IValueNode Node { get; set; }
    public Logger Logger { get; set; }
    public ValueNodeInterpreter(INode node, Logger logger)
    {
        Node = node as IValueNode;
        Logger = logger;
        Logger.Log($"Created {this.GetType().Name}", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        return new Number(Node.Value.Value);
    }
}