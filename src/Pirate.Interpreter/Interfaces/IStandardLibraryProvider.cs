using Pirate.Interpreter.Values.Function;

namespace Pirate.Interpreter.Interfaces;

public interface IStandardLibraryProvider
{
    List<CSharpFunction> GetFunction(string name);
}