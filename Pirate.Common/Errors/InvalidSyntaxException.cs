namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for invalid syntax errors.
/// </summary>
public class InvalidSyntaxException : System.Exception
{
    public InvalidSyntaxException() { }
    public InvalidSyntaxException(string message) : base(message) { }
    protected InvalidSyntaxException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}