using System.Runtime.Serialization;

namespace Pirate.Common.Exception.Exceptions;

/// <summary>
/// This is a custom exception for errors in the parser.
/// </summary>
public class ParserException : System.Exception
{
    public ParserException() { }
    public ParserException(string message) : base(message) { }


    protected ParserException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

