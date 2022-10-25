using System.Runtime.Serialization;

namespace Common.Errors;

[Serializable]
public class FileException : Exception
{
    public FileException() { }
    public FileException(string message) : base(message) { }


    protected FileException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

