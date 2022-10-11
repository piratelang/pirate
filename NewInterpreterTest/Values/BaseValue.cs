using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewInterpreterTest.Values.Interfaces;
using NewPirateLexer.Tokens;

namespace NewInterpreterTest.Values;

public abstract class BaseValue : IValue
{
    public virtual object Value {get; set;}
    public abstract BaseValue OperatedBy(Token _operator, BaseValue other);

    public bool Matches(BaseValue other)
    {
        if (Value.GetType() != other.Value.GetType())
        {
            Console.WriteLine("Types dont match");
            return false;
        }
        if (Value == other.Value)
        {
            Console.WriteLine("Values don't match");
            return false;
        }
        return true;
    }
}