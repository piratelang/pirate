namespace Pirate.Common.Exception.Exceptions;

public class ModuleNotBuiltException : System.Exception
{
    public ModuleNotBuiltException(string moduleName) : base($"No build output found for \"{moduleName}\". Run `pirate build` first.")
    {
    }
}
