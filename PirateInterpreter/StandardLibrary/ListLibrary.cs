using PirateInterpreter.Values;

namespace PirateInterpreter.StandardLibrary;

public class ListLibrary
{
    private readonly ILogger Logger;

    public ListLibrary(ILogger logger)
    {
        Logger = logger;
    }

    public BaseValue Add(IList<BaseValue> parameters)
    {
        Logger.Log($"Add called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[1] is null) throw new ArgumentNullException("Second parameter must not be null.");
        list.Values.Add(parameters[1]);

        if(list.Value is not string) list.Value = list.Values.ToString();
        SymbolTable.Instance(Logger).SetBaseValue((string)list.Value, list);
        return list;
    }

    public BaseValue Remove(IList<BaseValue> parameters)
    {
        Logger.Log($"Remove called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[1] is null) throw new ArgumentNullException("Second parameter must not be null.");
        Console.WriteLine(parameters[1].Equals(list.Values[1]));
        var result = list.Values.Remove(parameters[1]);

        if (result) Logger.Log($"Removed {parameters[1]} from {list.Value}", LogType.INFO);

        if(list.Value is not string) list.Value = list.Values.ToString();
        SymbolTable.Instance(Logger).SetBaseValue((string)list.Value, list);
        return list;
    }

    public BaseValue Get(IList<BaseValue> parameters)
    {
        Logger.Log($"Get called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[1] is not IntegerValue index) throw new ArgumentException("Second parameter must be an integer.");
        if ((Int64)index.Value < 0 || (Int64)index.Value >= list.Values.Count) throw new IndexOutOfRangeException("Index out of range.");
        return list.Values[int.Parse(index.Value.ToString())];
    }
    
    public BaseValue Set(IList<BaseValue> parameters)
    {
        Logger.Log($"Set called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[1] is not IntegerValue index) throw new ArgumentException("Second parameter must be an integer.");
        if ((Int64)index.Value < 0 || (Int64)index.Value >= list.Values.Count) throw new IndexOutOfRangeException("Index out of range.");
        list.Values[int.Parse(index.Value.ToString())] = parameters[2];
        return list;
    }

    public BaseValue Contains(IList<BaseValue> parameters)
    {
        Logger.Log($"Contains called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[1] is null) throw new ArgumentNullException("Second parameter must not be null.");
        return new BooleanValue(list.Values.Contains(parameters[1]), Logger);
    }

    public BaseValue Size(IList<BaseValue> parameters)
    {
        Logger.Log($"Size called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        return new IntegerValue(list.Values.Count, Logger);
    }

    public BaseValue Clear(IList<BaseValue> parameters)
    {
        Logger.Log($"Clear called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        list.Values.Clear();
        return list;
    }

    public ListValue Zip(IList<BaseValue> parameters)
    {
        Logger.Log($"Zip called with {parameters.Count} parameters", LogType.INFO);
        if (parameters[0] is not ListValue list1)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list1 = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        if (parameters[0] is not ListValue list2)
        {
            if(parameters[0] is VariableValue variable)
            {
                if(variable.Value is not string) throw new ArgumentException("First parameter must be a list or a variable containing a list");
                list2 = (ListValue)SymbolTable.Instance(Logger).GetBaseValue((string)variable.Value);
            }
            else throw new ArgumentException("First parameter must be a list or a variable containing a list");
        }
        
        if (list1.Values.Count != list2.Values.Count) throw new ArgumentException("Lists must be of equal length.");
        
        var resultList = new List<BaseValue>();
        foreach (var value in list1.Values)
        {
            resultList.Add(new ListValue(new List<BaseValue> { value, list2.Values[list1.Values.IndexOf(value)] }, Logger));
        }

        return new ListValue(resultList, Logger);
    }
}