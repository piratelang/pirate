using Interpreter.Values.Interfaces;
using Interpreter.Interpreters;

namespace Interpreter.Values;

public class Variable : BaseValue, IValue
{
    public InterpreterFactory InterpreterFactory { get; set; }
    public Variable(string value, ILogger logger, InterpreterFactory interpreterFactory): base(value, logger)
    {
        InterpreterFactory = interpreterFactory;
        Value = GetValue(value);
    }

    public object GetValue(string value)
    {
        var resultNode = SymbolTable.Instance(Logger).Get(value);
        if (resultNode is not IValueNode)
        {
           var interpreter = InterpreterFactory.GetInterpreter(resultNode, Logger);
           var result = interpreter.VisitNode();
           return result.Value;
        }
        else
        {
            var valueNode = (IValueNode)resultNode;
            return valueNode.Value.Value;
        }
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (Value.GetType())
        {
            case Type intType when Value.GetType() == typeof(int):
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