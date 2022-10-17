using System.Runtime.Serialization;

namespace Common.Errors;

[Serializable]
public class TypeConversionException : Exception
{
    public Type OrginType { get; set; }
    public Type TargetType { get; set; }
    public TypeConversionException() { }
    public TypeConversionException(string message) : base(message) { }
    public TypeConversionException(string message, Exception inner) : base(message, inner) { }
    public TypeConversionException(string message, Type orginType, Type targetType) : base(message) 
    { 
        OrginType = orginType;
        TargetType = targetType;
    }

    protected TypeConversionException(
        SerializationInfo info,
        StreamingContext context) : base(info, context) { }
}

