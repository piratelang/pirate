using PirateInterpreter.Interpreters.Interfaces;
using PirateInterpreter.Values;

namespace PirateInterpreter.Interpreters;

public class ForeachLoopStatementInterpreter : BaseInterpreter
{
    private IForeachLoopStatementNode ForeachLoopStatementNode { get; set; }

    public ForeachLoopStatementInterpreter(INode node, ILogger logger, InterpreterFactory interpreterFactory) : base(logger, interpreterFactory)
    {
        if (node is not IForeachLoopStatementNode) throw new TypeConversionException(typeof(IForeachLoopStatementNode), node.GetType());
        ForeachLoopStatementNode = (ForeachLoopStatementNode)node;

    }
    public override List<BaseValue> VisitNode()
    {
        INode node;

        //get list from symboltable
        ListValue listValue = GetList();

        if (ForeachLoopStatementNode.VariableAssign.Identifier.Value.Value is not string) throw new TypeConversionException(typeof(string), ForeachLoopStatementNode.VariableAssign.Identifier.Value.Value.GetType());
        List<BaseValue> returnValues = ExecuteBodyNodes(listValue);
        return returnValues;
    }

    private ListValue GetList()
    {
        if (ForeachLoopStatementNode.Value.Value.Value is not string) throw new TypeConversionException(typeof(string), ForeachLoopStatementNode.Value.Value.GetType());
        var list = SymbolTable.Instance(Logger).GetBaseValue((string)ForeachLoopStatementNode.Value.Value.Value);
        if (list is not ListValue) throw new TypeConversionException(typeof(ListValue), list.GetType());
        var listValue = (ListValue)list;
        return listValue;
    }

    private List<BaseValue> ExecuteBodyNodes(ListValue listValue)
    {
        List<BaseValue> returnValues = new List<BaseValue>();
        foreach (var item in listValue.Values)
        {
            SymbolTable.Instance(Logger).SetBaseValue((string)ForeachLoopStatementNode.VariableAssign.Identifier.Value.Value, item);

            //visit body
            foreach (var bodyNode in ForeachLoopStatementNode.Nodes)
            {
                var interpreter = InterpreterFactory.GetInterpreter(bodyNode);
                var result = interpreter.VisitNode();

                if (result is not null) returnValues.AddRange(result);
            }
        }

        return returnValues;
    }
}