using Pirate.Interpreter.Values;

namespace Pirate.Interpreter.StandardLibrary.Interfaces;

/// <inheritdoc cref="StandardLibraryCallManager"/>
public interface IStandardLibraryCallManager
{
    BaseValue CallFunction(string libraryname, string functionName, List<BaseValue> parameters);
}
