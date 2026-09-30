using Pirate.Common.FileHandler.Enum;

namespace Pirate.Common.FileHandler;

/// <summary>
/// This class is a base class for all file handlers.
/// </summary>
public class BaseFileHandler
{
    public string GetFileExtension(FileExtension fileExtension)
    {
        return "." + fileExtension.ToString().ToLower();
    }
}
