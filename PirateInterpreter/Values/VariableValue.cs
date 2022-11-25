using PirateInterpreter.Values.Interfaces;
using PirateInterpreter.Interpreters;

namespace PirateInterpreter.Values;

public class VariableValue : BaseValue, IValue
{
    public InterpreterFactory InterpreterFactory { get; set; }
    public VariableValue(string value, ILogger logger, InterpreterFactory interpreterFactory): base(value, logger)
    {
        InterpreterFactory = interpreterFactory;
        Value = SymbolTable.Instance(Logger).GetBaseValue(value).Value;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        Logger.Log($"Variable {Value.ToString()}, {Value.GetType()} is being operated by {other.ToString()}, {other.GetType()} with {_operator.ToString()}", LogType.INFO);
        switch (Value.GetType())
        {
            case Type intType when Value.GetType()== typeof(int):
            case Type when Value.GetType()== typeof(Int64):
            case Type when Value.GetType()== typeof(Int32):
                return new IntegerValue(Value, Logger).OperatedBy(_operator, other);
            case Type stringType when Value.GetType() == typeof(string):
                return new StringValue(Value, Logger).OperatedBy(_operator, other);
            case Type floatType when Value.GetType() == typeof(float):
                return new FloatValue(Value, Logger).OperatedBy(_operator, other);
            case Type charType when Value.GetType() == typeof(char):
                return new CharValue(Value, Logger).OperatedBy(_operator, other);
        }
        throw new NotImplementedException("No TypeCode found");
    }
}