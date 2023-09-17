using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

public class ExternInterpreter : BaseInterpreter
{
    private IExternNode _node;
    private IRuntime _runtime;
    private IStandardLibraryProvider _standardLibraryProvider;

    public ExternInterpreter(INode node, IRuntime runtime, IStandardLibraryProvider standardLibraryProvider, ILogger logger, InterpreterFactory interpreterFactory) : base(logger, interpreterFactory)
    {
        if (node is not IExternNode) throw new TypeConversionException(node.GetType(), typeof(IExternNode));
        _node = (IExternNode)node;
        _runtime = runtime;
        _standardLibraryProvider = standardLibraryProvider;
    }

    public override List<BaseValue> VisitNode()
    {
        var name = (string)_node.FunctionIdentifier.Value.Value;
        var functions = _standardLibraryProvider.GetFunction(name);

        foreach (var function in functions)
        {
            var functionName = function.Name.Split('.').Last();
            _runtime.Functions.Set(functionName, function);
        }

        return new();
    }
}
