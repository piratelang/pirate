using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.Interpreters;

/// <summary>
/// An interpreter for comments which does nothing.
/// </summary>
public class CommentInterpreter : BaseInterpreter
{
    public CommentInterpreter(INode node, InterpreterFactory interpreterFactory, ILogger logger) : base(logger, interpreterFactory)
    {
    }

    public override List<BaseValue> VisitNode()
    {
        return new List<BaseValue>();
    }
}