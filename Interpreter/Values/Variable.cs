using System;
using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;
using Parser.Node.Interfaces;

namespace Interpreter.Values;

public class Variable : BaseValue, IValue
{
    public IValueNode ValueNode { get; set; }
    public override object Value { get ; set; }
    public ILogger Logger { get; set; }

    public Variable(string value, ILogger logger)
    {
        ValueNode = (IValueNode) SymbolTable.Instance(logger).Get(value);
        Value= ValueNode.Value.Value;
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (Type.GetTypeCode(Value.GetType()))
        {
            case TypeCode.Int32:
                return new Integer(Value).OperatedBy(_operator, other);
            case TypeCode.String:
                return new String(Value, Logger).OperatedBy(_operator, other);
            case TypeCode.Double:
                return new Float(Value).OperatedBy(_operator, other);
            case TypeCode.Char:
                return new Char(Value, Logger).OperatedBy(_operator, other);
        }
        Logger.Log("No TypeCode found", this.GetType().Name, Common.Enum.LogType.ERROR);
        return null;
    }
}