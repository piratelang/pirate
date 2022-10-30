namespace PirateLexer;

public class Error
{
    public string errorName { get; set; }
    public string details { get; set; }

    public Error(string ErrorName, string Details)
    {
        errorName = ErrorName;
        details = Details;
    }

    public string AsString()
    {
        var result = $"{errorName}: {details}";
        return result;
    }
}
