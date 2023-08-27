using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Exception;
using Pirate.Common.FileHandler.Interfaces;

namespace Pirate.Common.FileHandler;

/// <summary>
/// This class handles reading files.
/// </summary>
public class FileReadHandler : BaseFileHandler, IFileReadHandler
{
    /// <summary>
    /// Reads all the text from a file.
    /// </summary>
    /// <param name="name">The name of the file</param>
    /// <param name="extension">The extension of the file</param>
    /// <param name="location">The location of the file</param>
    /// <returns>The text from the file</returns>
    /// <exception cref="FileHandlerException">Thrown when the name is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the location is null or empty</exception>
    /// <exception cref="FileNotFoundException">Thrown when the file could not be found</exception>
    public async Task<string> ReadAllTextFromFile(string name, FileExtension extension, string location)
    {
        if (name == string.Empty) throw new FileHandlerException($"Name, ${name} is empty");
        if (location == string.Empty) throw new FileHandlerException($"Location, ${location} is empty");

        var nameAndExtension = name + GetFileExtension(extension);
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);
        string fileName = Path.Combine(targetFolder, nameAndExtension);

        return FileExists(name, extension, location)
            ? await File.ReadAllTextAsync(fileName)
            : throw new FileNotFoundException("File not found");
    }

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    /// <param name="name">The name of the file</param>
    /// <param name="extension">The extension of the file</param>
    /// <param name="location">The location of the file</param>
    /// <returns>True if the file exists</returns>
    /// <exception cref="FileHandlerException">Thrown when the name is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the location is null or empty</exception>
    public bool FileExists(string name, FileExtension extension, string location)
    {
        if (name == string.Empty) throw new FileHandlerException($"Name, ${name} is empty");
        if (location == string.Empty) throw new FileHandlerException($"Location, ${location} is empty");

        name += GetFileExtension(extension);
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);
        string fileName = Path.Combine(targetFolder, name);

        return File.Exists(fileName);
    }

    /// <summary>
    /// Checks if a directory exists.
    /// </summary>
    /// <param name="location">The location of the directory</param>
    /// <returns>True if the directory exists</returns>
    /// <exception cref="FileHandlerException">Thrown when the location is null or empty</exception>
    public bool DirectoryExists(string location)
    {
        if (location == string.Empty) throw new FileHandlerException($"Location, ${location} is empty");

        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);

        return Directory.Exists(targetFolder);
    }
}