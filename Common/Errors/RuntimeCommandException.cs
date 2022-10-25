using System.Runtime.Serialization;

namespace Common.Errors;

[Serializable]
public class RuntimeCommandException : Exception
{
    public RuntimeCommandException() { }
    public RuntimeCommandException(string message) : base(message) { }


    protected RuntimeCommandException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

