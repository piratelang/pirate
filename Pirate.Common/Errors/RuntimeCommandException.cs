using System.Runtime.Serialization;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for runtime command errors.
/// </summary>
public class RuntimeCommandException : System.Exception
{
    public RuntimeCommandException() { }
    public RuntimeCommandException(string message) : base(message) { }


    protected RuntimeCommandException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

