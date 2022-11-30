using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ListDeclarationInterpreter : BaseInterpreter
{
    public IListDeclarationNode ListDeclarationNode { get; set; }

    public ListDeclarationInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IListDeclarationNode) throw new TypeConversionException(node.GetType(), typeof(IListDeclarationNode));
        ListDeclarationNode = (IListDeclarationNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{ListDeclarationNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{ListDeclarationNode.ToString()}\"", LogType.INFO);

        List<BaseValue> resultValues = GetValues();
        return new List<BaseValue>() { new ListValue(resultValues, Logger) };
    }

    private List<BaseValue> GetValues()
    {
        var resultValues = new List<BaseValue>();
        foreach (var item in ListDeclarationNode.Nodes)
        {
            var interpreter = InterpreterFactory.GetInterpreter(item);
            var itemValue = interpreter.VisitSingleNode();
            resultValues.Add(itemValue);
        }

        return resultValues;
    }
}