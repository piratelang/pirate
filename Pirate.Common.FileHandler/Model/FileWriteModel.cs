using System.ComponentModel.DataAnnotations;
using Pirate.Common.FileHandler.Enum;

namespace Pirate.Common.FileHandler.Model;

/// <summary>
/// This is a model for the file handlers.
/// </summary>
public class FileWriteModel
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string Location { get; set; }
    public string Text { get; set; }

    /// <summary>
    /// Creates a new FileWriteModel
    /// </summary>
    /// <param name="name">Name of the file without extension</param>
    /// <param name="extension">Extension of the file with dot</param>
    /// <param name="location">Location of the file without root folder, i.e. without "./"</param>
    /// <param name="text">Text to write to the file</param>
    public FileWriteModel(string fileName, FileExtension fileExtension, string fileLocation, string fileText)
    {
        Name = fileName;
        Extension = "." + fileExtension.ToString().ToLower();
        Location = fileLocation;
        Text = fileText;
    }

    internal bool IsValid()
    {
        if (string.IsNullOrEmpty(Name)) throw new ValidationException($"Name, ${Name} is empty");
        if (string.IsNullOrEmpty(Extension)) throw new ValidationException($"Extension, ${Extension} is empty");
        // if (string.IsNullOrEmpty(Location)) throw new ValidationException($"Location, ${Location} is empty");
        if (string.IsNullOrEmpty(Text)) throw new ValidationException($"Text, ${Text} is empty");

        return true;
    }
}