namespace PirateInterpreter.Values;

public class ListValue : BaseValue
{
    public List<BaseValue> Values { get; set; }

    public ListValue(List<BaseValue> values, ILogger logger) : base("List", logger)
    {
        Values = values;
        Logger.Log($"Created {this.GetType().Name} : \"{Values.ToString()}\"", LogType.INFO);
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        switch (_operator.TokenType)
        {
            case TokenType.PLUS:
                if (other is not ListValue) throw new TypeConversionException(other.GetType(), typeof(ListValue));
                var otherList = (ListValue)other;
                var newList = new List<BaseValue>();
                newList.AddRange(Values);
                newList.AddRange(otherList.Values);
                return new ListValue(newList, Logger);
            default:
                throw new InvalidOperationException($"Cannot operate {this.GetType().Name} by {_operator.ToString()}");
        }
    }

    public BaseValue GetItem(Int64 index)
    {
        var _index = Convert.ToInt32(index);
        if (index >= Values.Count) throw new IndexOutOfRangeException($"Index {index} is out of range");
        return Values[_index];
    }
}