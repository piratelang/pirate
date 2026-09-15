using Pirate.Common.Exception;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for errors in the parser.
/// </summary>
public class ParserException : PirateException
{
    public ParserException(ExceptionCode code) : base(code) { }
    public ParserException(ExceptionCode code, System.Exception inner) : base(code, inner) { }
    public ParserException(ExceptionCode code, List<string> parameters) : base(code, parameters) { }
    public ParserException(ExceptionCode code, List<string> parameters, System.Exception inner) : base(code, parameters, inner) { }
}
