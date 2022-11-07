namespace Common.Interfaces;

public interface IFileReadHandler
{
    bool FileExists(string name, string extension, string location);
    string ReadAllTextFromFile(string name, string extension, string location);
}
