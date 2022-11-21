using PirateInterpreter.StandardLibrary;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class FunctionCallInterpreter : BaseInterpreter
{
    public IFunctionCallNode functionCallNode { get; set; }
    private StandardLibraryFactory StandardLibraryFactory;

    public FunctionCallInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, StandardLibraryFactory standardLibraryFactory) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionCallNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionCallNode));
        functionCallNode = (IFunctionCallNode)node;
        StandardLibraryFactory = standardLibraryFactory;

        Logger.Log($"Created {this.GetType().Name} : \"{functionCallNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{functionCallNode.ToString()}\"", this.GetType().Name, Common.Enum.LogType.INFO);

        var functionValue = SymbolTable.Instance(Logger).GetBaseValue((string)functionCallNode.Identifier.Value.Value);
        if (functionValue is not FunctionValue) throw new TypeConversionException(functionValue.GetType(), typeof(FunctionValue));
        var function = (FunctionValue)functionValue;

        if (function.functionDeclarationNode == null)
        {
            if (function.functionDeclarationNode.Identifier.Value.Value is not string) throw new TypeConversionException(function.functionDeclarationNode.Identifier.Value.Value.GetType(), typeof(string));
            List<BaseValue> parameters = new List<BaseValue>();
            foreach (var parameter in functionCallNode.Parameters)
            {
                var parameterInterpreter = InterpreterFactory.GetInterpreter(parameter, Logger);
                parameters.AddRange(parameterInterpreter.VisitNode());
            }
            StandardLibraryFactory.GetFunction((string)function.functionDeclarationNode.Identifier.Value.Value, parameters);
        }

        foreach (var (parameter, value) in function.functionDeclarationNode.Parameters.Zip(functionCallNode.Parameters))
        {
            SymbolTable.Instance(Logger).SetBaseValue((string)parameter.Identifier.Value.Value, InterpreterFactory.GetInterpreter(value, Logger).VisitSingleNode());
        }

        foreach (var node in function.functionDeclarationNode.Statements)
        {
            var interpreter = InterpreterFactory.GetInterpreter(node, Logger);
            var values = interpreter.VisitNode();
        }

        var resultList = new List<BaseValue>();
        if (function.functionDeclarationNode.ReturnNode is not null)
        {
            resultList.Add(InterpreterFactory.GetInterpreter(function.functionDeclarationNode.ReturnNode, Logger).VisitSingleNode());
        }
        return resultList;
    }
}