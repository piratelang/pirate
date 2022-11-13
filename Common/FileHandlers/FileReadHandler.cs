using System.Globalization;
using Common.Enum;
using Common.FileHandlers.Interfaces;

namespace Common.FileHandlers;

[Serializable]
public class FileReadHandler : IFileReadHandler
{
    public async Task<string> ReadAllTextFromFile(string name, FileExtension extension, string location)
    {
        if (name == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        var nameAndExtension = name + "." + extension.ToString().ToLower();
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);
        string fileName = Path.Combine(targetFolder, nameAndExtension);

        if (FileExists(name, extension, location))
        {
            return await File.ReadAllTextAsync(fileName);
        }
        else
        {
            throw new FileNotFoundException("File not found");
        }
    }

    public bool FileExists(string name, FileExtension extension, string location)
    {
        if (name == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        name += "." + extension.ToString().ToLower();
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);
        string fileName = Path.Combine(targetFolder, name);

        return File.Exists(fileName);
    }

    public bool DirectoryExists(string location)
    {
        if (location == string.Empty ) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);

        return Directory.Exists(targetFolder);
    }
}