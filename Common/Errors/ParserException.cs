using System.Runtime.Serialization;

namespace Common.Errors;

[Serializable]
public class ParserException : Exception
{
    public ParserException() { }
    public ParserException(string message) : base(message) { }


    protected ParserException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

