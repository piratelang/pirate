using System.Runtime.Serialization;

namespace PirateLexer.Tokens;

[Serializable]
internal class TyepKeyWorkNotImplementedException : Exception
{
    public TyepKeyWorkNotImplementedException()
    {
    }

    public TyepKeyWorkNotImplementedException(string message) : base(message)
    {
    }

    public TyepKeyWorkNotImplementedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected TyepKeyWorkNotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}