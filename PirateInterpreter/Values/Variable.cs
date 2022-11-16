using PirateInterpreter.Values.Interfaces;
using PirateInterpreter.Interpreters;

namespace PirateInterpreter.Values;

public class Variable : BaseValue, IValue
{
    public InterpreterFactory InterpreterFactory { get; set; }
    public Variable(string value, ILogger logger, InterpreterFactory interpreterFactory): base(value, logger)
    {
        InterpreterFactory = interpreterFactory;
        Value = SymbolTable.Instance(Logger).GetBaseValue(value).Value;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        Logger.Log($"Variable {Value.ToString()}, {Value.GetType()} is being operated by {other.ToString()}, {other.GetType()} with {_operator.ToString()}", this.GetType().Name, LogType.INFO);
        switch (Value.GetType())
        {
            case Type intType when Value.GetType()== typeof(int):
                return new Integer(Value, Logger).OperatedBy(_operator, other);
            case Type stringType when Value.GetType() == typeof(string):
                return new String(Value, Logger).OperatedBy(_operator, other);
            case Type floatType when Value.GetType() == typeof(float):
                return new Float(Value, Logger).OperatedBy(_operator, other);
            case Type charType when Value.GetType() == typeof(char):
                return new Char(Value, Logger).OperatedBy(_operator, other);
        }
        throw new NotImplementedException("No TypeCode found");
    }
}