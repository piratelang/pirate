namespace Pirate.Common.Logger.Exception;

using System;

public class LoggerException : Exception
{
    public LoggerException(string message) : base(message) { }
}