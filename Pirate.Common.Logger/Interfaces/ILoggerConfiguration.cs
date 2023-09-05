using Pirate.Common.Logger.Enum;

namespace Pirate.Common.Logger.Interfaces;

public partial interface ILoggerConfiguration
{
    string FolderName { get; set; }

    UseConsoleEnum UseConsole { get; set; }
    UseFileEnum UseFile { get; set; }
}
