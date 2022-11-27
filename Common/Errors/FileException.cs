using System.Runtime.Serialization;

namespace Common.Errors;

/// <summary>
/// This is a custom exception for file errors.
/// </summary>
public class FileException : Exception
{
    public FileException() { }
    public FileException(string message) : base(message) { }


    protected FileException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

