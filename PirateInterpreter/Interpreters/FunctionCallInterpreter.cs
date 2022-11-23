using PirateInterpreter.StandardLibrary;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class FunctionCallInterpreter : BaseInterpreter
{
    public IFunctionCallNode functionCallNode { get; set; }

    private StandardLibraryFactory StandardLibraryFactory;
    private List<string> LibraryList = new() { "IO" };

    public FunctionCallInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, StandardLibraryFactory standardLibraryFactory) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionCallNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionCallNode));
        functionCallNode = (IFunctionCallNode)node;
        StandardLibraryFactory = standardLibraryFactory;

        Logger.Log($"Created {this.GetType().Name} : \"{functionCallNode.ToString()}\"", Common.Enum.LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{functionCallNode.ToString()}\"", Common.Enum.LogType.INFO);

        var functionCallName = (string)functionCallNode.Identifier.Value.Value;
        var splitFunctionCallName = functionCallName.Split(".");
        if (LibraryList.Contains(splitFunctionCallName[0]))
        {
            return CallLibraryFunction(splitFunctionCallName, functionCallName);
        }

        return CallDeclaredFunction();
    }

    private List<BaseValue> CallDeclaredFunction()
    {
        var foundFunctionValue = SymbolTable.Instance(Logger).GetBaseValue((string)functionCallNode.Identifier.Value.Value);
        if (foundFunctionValue is not FunctionValue) throw new TypeConversionException(foundFunctionValue.GetType(), typeof(FunctionValue));
        var foundFunction = (FunctionValue)foundFunctionValue;

        foreach (var (parameter, value) in foundFunction.functionDeclarationNode.Parameters.Zip(functionCallNode.Parameters))
        {
            var parameterName = (string)parameter.Identifier.Value.Value;
            var parameterValue = InterpreterFactory.GetInterpreter(value).VisitSingleNode();
            SymbolTable.Instance(Logger).SetBaseValue(parameterName, parameterValue);
        }

        foreach (var node in foundFunction.functionDeclarationNode.Statements)
        {
            var interpreter = InterpreterFactory.GetInterpreter(node);
            interpreter.VisitNode();
        }

        var resultList = new List<BaseValue>();
        if (foundFunction.functionDeclarationNode.ReturnNode is not null)
        {
            var result = InterpreterFactory.GetInterpreter(foundFunction.functionDeclarationNode.ReturnNode).VisitSingleNode();
            resultList.Add(result);
        }
        return resultList;
    }

    private List<BaseValue> CallLibraryFunction(string[] splitidentifier, string functionCallName)
    {
        var libraryValue = SymbolTable.Instance(Logger).GetBaseValue(splitidentifier[0]);
        if (splitidentifier.Count() > 2) throw new InvalidOperationException("Cannot call a function in a library in a library");
        var library = (Library)libraryValue;
        List<BaseValue> parameters = new();
        foreach (var parameter in functionCallNode.Parameters)
        {
            var parameterInterpreter = InterpreterFactory.GetInterpreter(parameter);
            parameters.AddRange(parameterInterpreter.VisitNode());
        }
        return new List<BaseValue>() { StandardLibraryFactory.CallFunction(splitidentifier[0], splitidentifier[1], parameters, Logger) };
    }
}