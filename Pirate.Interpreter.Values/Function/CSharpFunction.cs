using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pirate.Interpreter.Values.Function;

public abstract class CSharpFunction : BaseValue, IFunctionValue
{
    public CSharpFunction(object? value, ILogger logger) : base(value, logger)
    {
    }

    public override BaseValue OperatedBy(Token _operator, BaseValue other)
    {
        throw new InvalidOperationException($"Cannot operate {GetType().Name} by {_operator}");
    }

    public abstract object Execute(List<object> arguments);
}
