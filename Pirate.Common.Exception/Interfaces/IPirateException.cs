using Pirate.Common.Exception.Models;

namespace Pirate.Common.Exception.Interfaces;

public interface IPirateException
{
    public ExceptionCode Code { get; set; }
}
