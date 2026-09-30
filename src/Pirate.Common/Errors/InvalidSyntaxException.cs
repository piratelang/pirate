using Pirate.Common.Exception;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for invalid syntax errors.
/// </summary>
public class InvalidSyntaxException : PirateException
{
    public InvalidSyntaxException(ExceptionCode code) : base(code) { }
    public InvalidSyntaxException(ExceptionCode code, System.Exception inner) : base(code, inner) { }
    public InvalidSyntaxException(ExceptionCode code, List<string> parameters) : base(code, parameters) { }
    public InvalidSyntaxException(ExceptionCode code, List<string> parameters, System.Exception inner) : base(code, parameters, inner) { }
}