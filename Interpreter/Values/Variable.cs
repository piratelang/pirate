using System;
using Interpreter.Values.Interfaces;
using Lexer.Enums;
using Lexer.Tokens;
using Common;
using Parser.Node.Interfaces;
using Interpreter.Interpreters;
using Parser.Node;

namespace Interpreter.Values;

public class Variable : BaseValue, IValue
{
    public IValueNode? ValueNode { get; set; }
    public override object Value { get ; set; }
    public ILogger Logger { get; set; }

    public Variable(string value, ILogger logger, InterpreterFactory interpreterFactory)
    {
        var ResultNode = SymbolTable.Instance(logger).Get(value);
        if (ResultNode is not IValueNode)
        {
           var interpreter = interpreterFactory.GetInterpreter(ResultNode, logger);
           var result = interpreter.VisitNode();
           Value = result.Value;
        }
        else
        {
            ValueNode = (IValueNode)SymbolTable.Instance(logger).Get(value);
            Value= ValueNode.Value.Value;
        }
        Logger = logger;
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (Value.GetType())
        {
            case Type intType when Value.GetType() == typeof(int):
                return new Integer(Value).OperatedBy(_operator, other);
            case Type stringType when Value.GetType() == typeof(string):
                return new String(Value, Logger).OperatedBy(_operator, other);
            case Type floatType when Value.GetType() == typeof(float):
                return new Float(Value).OperatedBy(_operator, other);
            case Type charType when Value.GetType() == typeof(char):
                return new Char(Value, Logger).OperatedBy(_operator, other);
        }
        Logger.Log("No TypeCode found", this.GetType().Name, Common.Enum.LogType.ERROR);
        throw new NotImplementedException("No TypeCode found");
    }
}