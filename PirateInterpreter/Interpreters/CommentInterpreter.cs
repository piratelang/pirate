using PirateInterpreter.Interpreters.Interfaces;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

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