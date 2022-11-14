using Common.Enum;

namespace Common.FileHandlers;

public class BaseFileHandler
{
    public string GetFileExtension(FileExtension fileExtension)
    {
        return "." + fileExtension.ToString().ToLower();
    }
}
