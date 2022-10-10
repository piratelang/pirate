using NewPirateLexer.Tokens;

namespace NewInterpreterTest.Values.Interfaces;

public interface INumber : IValue
{
    int Value {get;set;}
    IValue OperatedBy(Token _operator, INumber number);
}