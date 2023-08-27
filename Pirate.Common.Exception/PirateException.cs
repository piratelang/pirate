using System.Resources;
using Pirate.Common.Exception.Interfaces;

namespace Pirate.Common.Exception.Models;

public abstract class PirateException : System.Exception, IPirateException
{
    public ExceptionCode Code { get; set; }

    public PirateException(ExceptionCode code) : base(GetFullMessage(code))
    {
        Code = code;
    }
    
    public PirateException(ExceptionCode code, System.Exception innerException) : base(GetFullMessage(code), innerException)
    {
        Code = code;
    }

    public PirateException(ExceptionCode code, List<string> parameters) : base(GetFullMessage(code, parameters))
    {
        Code = code;
    }

    public PirateException(ExceptionCode code, List<string> parameters, System.Exception innerException) : base(GetFullMessage(code, parameters), innerException)
    {
        Code = code;
    }

    
    private static string GetFullMessage(ExceptionCode code, List<string>? parameters = null)
    {
        ResourceManager resourceManager = new("Pirate.Common.Exception.ExceptionMessages", typeof(PirateException).Assembly);
        var message = resourceManager.GetString(code.GetFullCode()) ?? "Unknown error";
        if (parameters != null)
            message = string.Format(message, parameters?.ToArray());

        return $"{code.Prefix}{code.Code}: {message}";
    }
}