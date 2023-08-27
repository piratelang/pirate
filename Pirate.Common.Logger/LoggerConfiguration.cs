using Pirate.Common.Logger.Enum;
using Pirate.Common.Logger.Interfaces;

namespace Pirate.Common.Logger;

/// <summary>
/// The configuration options for the logger.
/// </summary>
public class LoggerConfiguration : ILoggerConfiguration
{

    /// <summary>
    /// The name of the folder where the logs will be stored.
    /// </summary>
    public string FolderName { get; set; } = ".logs";


    /// <summary>
    /// If the logger should use the console.
    /// </summary>
    public UseConsoleEnum UseConsole { get; set; } = UseConsoleEnum.False;



    /// <summary>
    /// If the logger should use a file.
    /// </summary>
    public UseFileEnum UseFile { get; set; } = UseFileEnum.True;


}
