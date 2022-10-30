using System.Runtime.Serialization;

namespace PirateLexer.Tokens
{
    [Serializable]
    internal class ControlKeyWorkNotImplementedException : Exception
    {
        public ControlKeyWorkNotImplementedException()
        {
        }

        public ControlKeyWorkNotImplementedException(string message) : base(message)
        {
        }

        public ControlKeyWorkNotImplementedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ControlKeyWorkNotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}