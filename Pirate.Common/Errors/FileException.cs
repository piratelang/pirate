using Pirate.Common.Exception.Models;
using System.Runtime.Serialization;

namespace Pirate.Common.Errors;

/// <summary>
/// This is a custom exception for file errors.
/// </summary>
public class FileException : PirateException
{
    public FileException(ExceptionCode code) : base(code) { }
    public FileException(ExceptionCode code, System.Exception inner) : base(code, inner) { }
    public FileException(ExceptionCode code, List<string> parameters) : base(code, parameters) { }
    public FileException(ExceptionCode code, List<string> parameters, System.Exception inner) : base(code, parameters, inner) { }

}

