namespace Common.FileHandlers.Interfaces;

public interface IFileReadHandler
{
    bool FileExists(string name, string extension, string location);
    Task<string> ReadAllTextFromFile(string name, string extension, string location);
}
