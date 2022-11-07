using System.Runtime.InteropServices.ComTypes;
using Common.Interfaces;

namespace Common;

[Serializable]
public class FileReadHandler : IFileReadHandler
{

    // Name: Without extension
    // Extension: With! dot
    // Location: Without root folder, i.e. "./"
    public string ReadAllTextFromFile(string name, string extension, string location)
    {
        if (name == string.Empty || extension == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        name += extension;
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);
        string fileName = Path.Combine(targetFolder, name);

        return File.ReadAllText(fileName);
    }

    public bool FileExists(string name, string extension, string location)
    {
        if (name == string.Empty || extension == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        name += extension;
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