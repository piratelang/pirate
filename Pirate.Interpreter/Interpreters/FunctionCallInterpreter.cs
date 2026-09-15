using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;
using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// This class is an interpreter for function calls.
/// It looks for the function in the standard library.
/// Otherwise it looks for the declared function.
/// </summary>
public class FunctionCallInterpreter : BaseInterpreter
{
    public IFunctionCallNode functionCallNode { get; set; }

    private IStandardLibraryProvider _standardLibraryFactory;
    private IRuntime _runtime;

    public FunctionCallInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, IStandardLibraryProvider standardLibraryFactory, IRuntime runtime) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionCallNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionCallNode));
        functionCallNode = (IFunctionCallNode)node;
        _standardLibraryFactory = standardLibraryFactory;
        _runtime = runtime;

        Logger.Info($"Created {GetType().Name} : \"{functionCallNode.ToString()}\"");
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Info($"Visiting {GetType().Name} : \"{functionCallNode.ToString()}\"");

        var functionCallName = (string)functionCallNode.Identifier.Value.Value;

        return CallDeclaredFunction();
    }

    private List<BaseValue> CallDeclaredFunction()
    {
        var foundFunctionValue = _runtime.Functions.Get((string)functionCallNode.Identifier.Value.Value);
        if (foundFunctionValue is not IFunctionValue) throw new TypeConversionException(foundFunctionValue.GetType(), typeof(IFunctionValue));

        if (foundFunctionValue is FunctionValue)
        {
            var foundFunction = (FunctionValue)foundFunctionValue;
            SetVariables(foundFunction);

            foreach (var node in foundFunction.FunctionDeclarationNode.Statements)
            {
                var interpreter = InterpreterFactory.GetInterpreter(node);
                interpreter.VisitNode();
            }

            List<BaseValue> resultList = InterpretResultNode(foundFunction);
            return resultList;
        }
        else if (foundFunctionValue is CSharpFunction)
        {
            var foundFunction = (CSharpFunction)foundFunctionValue;
            var parameters = functionCallNode.Parameters.Select(x => InterpreterFactory.GetInterpreter(x).VisitSingleNode().Value).ToList();
            var result = foundFunction.Execute(parameters);
            return result;
        }
        else
        {
            throw new TypeConversionException(foundFunctionValue.GetType(), typeof(IFunctionValue));
        }

    }

    private List<BaseValue> InterpretResultNode(FunctionValue foundFunction)
    {
        var resultList = new List<BaseValue>();
        if (foundFunction.FunctionDeclarationNode.ReturnNode is not null)
        {
            var result = InterpreterFactory.GetInterpreter(foundFunction.FunctionDeclarationNode.ReturnNode).VisitSingleNode();
            resultList.Add(result);
        }

        return resultList;
    }

    private void SetVariables(FunctionValue foundFunction)
    {
        foreach (var (parameter, value) in foundFunction.FunctionDeclarationNode.Parameters.Zip(functionCallNode.Parameters))
        {
            var parameterName = (string)parameter.Identifier.Value.Value;
            var parameterValue = InterpreterFactory.GetInterpreter(value).VisitSingleNode();
            _runtime.Variables.Set(parameterName, parameterValue);
        }
    }
}