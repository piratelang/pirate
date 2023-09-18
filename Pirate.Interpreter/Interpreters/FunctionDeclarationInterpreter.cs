using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// Converts the function declaration node to a function value.
/// </summary>
public class FunctionDeclarationInterpreter : BaseInterpreter
{
    public IFunctionDeclarationNode FunctionDeclarationNode { get; set; }
    public IRuntime _runtime { get; set; }

    public FunctionDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger, IRuntime runtime) : base(logger, InterpreterFactory)
    {
        if (node is not IFunctionDeclarationNode) throw new TypeConversionException(node.GetType(), typeof(IFunctionDeclarationNode));
        FunctionDeclarationNode = (IFunctionDeclarationNode)node;

        Logger.Info($"Created {GetType().Name} : \"{FunctionDeclarationNode.ToString()}\"");
        _runtime = runtime;
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Info($"Visiting {GetType().Name} : \"{FunctionDeclarationNode.ToString()}\"");

        var function = new FunctionValue(FunctionDeclarationNode, Logger);
        _runtime.Functions.Set((string)FunctionDeclarationNode.Identifier.Value.Value, function);
        return new List<BaseValue>();
    }
}