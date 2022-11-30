using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ListAccessInterpreter : BaseInterpreter
{
    public IListAccessNode ListAccessNode { get; set; }

    public ListAccessInterpreter(INode node, InterpreterFactory InterpreterFactory, ILogger logger) : base(logger, InterpreterFactory)
    {
        if (node is not IListAccessNode) throw new TypeConversionException(node.GetType(), typeof(ListAccessNode));
        ListAccessNode = (IListAccessNode)node;

        Logger.Log($"Created {this.GetType().Name} : \"{ListAccessNode.ToString()}\"", LogType.INFO);
    }

    public override List<BaseValue> VisitNode()
    {
        Logger.Log($"Visiting {this.GetType().Name} : \"{ListAccessNode.ToString()}\"", LogType.INFO);

        //get list from symboltable
        if (ListAccessNode.Identifier.Value.Value is not string) throw new TypeConversionException(ListAccessNode.Identifier.Value.Value.GetType(), typeof(string));
        var list = SymbolTable.Instance(Logger).GetBaseValue((string)ListAccessNode.Identifier.Value.Value);
        if (list is not ListValue) throw new TypeConversionException(list.GetType(), typeof(ListValue));
        var listValue = (ListValue)list;

        var interpreter = InterpreterFactory.GetInterpreter(ListAccessNode.Index);
        var index = interpreter.VisitSingleNode();

        if (index is not IntegerValue) throw new TypeConversionException(index.GetType(), typeof(IntegerValue));

        var item = listValue.GetItem((Int64)((IntegerValue)index).Value);


        return new List<BaseValue>() { item };
    }
}