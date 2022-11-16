using Common.Enum;

namespace Common.FileHandlers;

[Serializable]
public class BaseFileHandler
{
    public string GetFileExtension(FileExtension fileExtension)
    {
        return "." + fileExtension.ToString().ToLower();
    }
}
