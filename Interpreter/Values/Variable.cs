using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;

namespace Interpreter.Values;

public class Variable : BaseValue, IValue
{
    public override object Value { get; set; }
    public Type Type { get; set; }
    public Logger Logger { get; set; }

    public Variable(object value, Logger logger)
    {
        Value = value;
        Type = Value.GetType();
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        BaseValue result;
        switch (Type.GetTypeCode(Type))
        {
            case TypeCode.Int32:
                result = new Integer(Value).OperatedBy(_operator, other);
                return new Variable(result.Value, Logger);
            case TypeCode.String:
                result = new String(Value, Logger).OperatedBy(_operator, other);
                return new Variable(result.Value, Logger);
            case TypeCode.Double:
                result = new Float(Value).OperatedBy(_operator, other);
                return new Variable(result.Value, Logger);
            case TypeCode.Char:
                result = new Char(Value, Logger).OperatedBy(_operator, other);
                return new Variable(result.Value, Logger);
        }
        Logger.Log("No TypeCode found", this.GetType().Name, Common.Enum.LogType.ERROR);
        return null;
    }
}