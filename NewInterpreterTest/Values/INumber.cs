using NewPirateLexer.Tokens;

namespace NewInterpreterTest.Values;

public interface INumber : IValue
{
    int Value {get;set;}
    IValue OperatedBy(Token _operator, INumber number);
}