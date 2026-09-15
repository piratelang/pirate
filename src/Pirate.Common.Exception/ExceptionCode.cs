namespace Pirate.Common.Exception;

public class ExceptionCode
{
    public ExceptionPrefix Prefix { get; set; }
    public string Code { get; set; }

    public ExceptionCode(string prefix, string code)
    {
        Prefix = Enum.Parse<ExceptionPrefix>(prefix);
        Code = code;
    }

    public string GetFullCode()
    {
        return $"{Prefix}-{Code}";
    }
}