using System.Runtime.Serialization;

namespace Common.Errors;

/// <summary>
/// This is a custom exception for runtime command errors.
/// </summary>
public class RuntimeCommandException : Exception
{
    public RuntimeCommandException() { }
    public RuntimeCommandException(string message) : base(message) { }


    protected RuntimeCommandException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

