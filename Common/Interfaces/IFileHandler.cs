namespace Common.Interfaces;

public interface IFileHandler
{
    bool AppendToFile(string name, string extension, string text, string location, bool encryption = false);
    bool WriteToFile(string name, string extension, string text, string location, bool encryption = false);
}
