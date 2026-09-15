namespace Pirate.Common.Exception;

public class ExceptionCode
{
    public ExceptionPrefix Prefix { get; set; }
    public string Code { get; set; }

    public ExceptionCode(ExceptionPrefix prefix, string code)
    {
        Prefix = prefix;
        Code = code;
    }

    public string GetFullCode()
    {
        return $"{Prefix}-{Code}";
    }
}