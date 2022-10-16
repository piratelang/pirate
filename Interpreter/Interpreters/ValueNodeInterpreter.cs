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
        Logger.Log($"Created {this.GetType().Name} : \"{Node.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override BaseValue VisitNode()
    {
        switch (Node.Value.TokenType)
        {
            case TokenValue.INT:
                return new Integer(Node.Value.Value);
            case TokenValue.STRING:
                return new Values.String(Node.Value.Value, Logger);
            case TokenValue.CHAR:
                return new Values.Char(Node.Value.Value, Logger);
            case TokenValue.FLOAT:
                return new Float(Node.Value.Value);
            case TokenSyntax.IDENTIFIER:
                return new Variable((string)Node.Value.Value, Logger);
        } 
        Logger.Log($"{Node.Value.GetType().Name} does not contain a vaild value type.", this.GetType().Name, Common.Enum.LogType.ERROR);
        return null;

    }
}