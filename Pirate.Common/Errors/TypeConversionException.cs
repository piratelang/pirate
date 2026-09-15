using Pirate.Common.Exception;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for type conversion errors.
/// </summary>
public class TypeConversionException : PirateException
{
    public Type? OrginType { get; set; }
    public Type? TargetType { get; set; }
    public TypeConversionException() : base(new ExceptionCode(ExceptionPrefix.INTERPRETER, "003"), new List<string> { "Type conversion failed" }) { }
    public TypeConversionException(string message) : base(new ExceptionCode(ExceptionPrefix.INTERPRETER, "003"), new List<string> { message }) { }
    public TypeConversionException(Type targetType) : base(new ExceptionCode(ExceptionPrefix.INTERPRETER, "001"), new List<string> { targetType.Name })
    {
        TargetType = targetType;
    }
    public TypeConversionException(Type orginType, Type targetType) : base(new ExceptionCode(ExceptionPrefix.INTERPRETER, "002"), new List<string> { orginType.Name, targetType.Name })
    {
        OrginType = orginType;
        TargetType = targetType;
    }
}
