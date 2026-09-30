namespace Pirate.Common.Logger.Enum;

public enum UseFileEnum
{
    /// <summary>
    /// The logger will use a file.
    /// </summary>
    True,
    /// <summary>
    /// The logger will not use a file.
    /// </summary>
    False,
    /// <summary>
    /// The logger will use a file if a fatal error occurs.
    /// </summary>
    OnFatal
}
