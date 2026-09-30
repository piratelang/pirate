using System.Runtime.Serialization;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for type conversion errors.
/// </summary>
public class TypeConversionException : System.Exception
{
    public Type? OrginType { get; set; }
    public Type? TargetType { get; set; }
    public TypeConversionException() { }
    public TypeConversionException(string message) : base(message) { }
    public TypeConversionException(Type targetType) : base($"Failed to convert to {targetType.Name}")
    {
        TargetType = targetType;
    }
    public TypeConversionException(Type orginType, Type targetType) : base($"Failed to convert {orginType.Name} to {targetType.Name}")
    {
        OrginType = orginType;
        TargetType = targetType;
    }

    protected TypeConversionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

