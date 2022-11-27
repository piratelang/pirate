using Common.Enum;

namespace Common.FileHandlers;

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
