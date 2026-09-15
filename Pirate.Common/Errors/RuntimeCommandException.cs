using Pirate.Common.Exception;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for runtime command errors.
/// </summary>
public class RuntimeCommandException : PirateException
{
    public RuntimeCommandException() : base(new ExceptionCode(ExceptionPrefix.SHELL, "002")) { }
    public RuntimeCommandException(string message) : base(new ExceptionCode(ExceptionPrefix.SHELL, "001"), new List<string> { message }) { }
}
