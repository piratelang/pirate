using Pirate.Common.FileHandler.Exception;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.FileHandler.Model;

namespace Pirate.Common.FileHandler;

/// <summary>
/// This class handles writing to files.
/// </summary>
public class FileWriteHandler : BaseFileHandler, IFileWriteHandler
{
    /// <summary>
    /// Writes text to a file.
    /// </summary>
    /// <param name="fileWriteModel">The file write model</param>
    /// <returns>True if the text was written successfully</returns>
    /// <exception cref="FileHandlerException">Thrown when the name is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the location is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the text is null or empty</exception>
    public bool WriteToFile(FileWriteModel fileWriteModel)
    {
        fileWriteModel.IsValid();

        CreateFolder(fileWriteModel.Location);

        var result = Write(fileWriteModel).Result;

        return result;
    }

    /// <summary>
    /// Appends text to a file.
    /// </summary>
    /// <param name="fileWriteModel">The file write model</param>
    /// <returns>True if the text was appended successfully</returns>
    /// <exception cref="FileHandlerException">Thrown when the name is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the location is null or empty</exception>
    /// <exception cref="FileHandlerException">Thrown when the text is null or empty</exception>
    public bool AppendToFile(FileWriteModel fileWriteModel)
    {
        fileWriteModel.IsValid();

        CreateFolder(fileWriteModel.Location);

        var result = AppendWrite(fileWriteModel).Result;

        return result;
    }

    private void CreateFolder(string location)
    {
        var targetFolder = Path.Combine(Environment.CurrentDirectory, location);

        bool exists = Directory.Exists(targetFolder);
        if (!exists)
        {
            Directory.CreateDirectory(targetFolder);
        }
    }

    private async Task<bool> Write(FileWriteModel fileWriteModel)
    {
        var name = fileWriteModel.Name + fileWriteModel.Extension;
        var targetFolder = Path.Combine(Environment.CurrentDirectory, fileWriteModel.Location);

        string fileName = Path.Combine(targetFolder, name);

        await File.WriteAllTextAsync(fileName, fileWriteModel.Text);

        return true;
    }

    private async Task<bool> AppendWrite(FileWriteModel fileWriteModel)
    {
        var name = fileWriteModel.Name + fileWriteModel.Extension;
        var targetFolder = Path.Combine(Environment.CurrentDirectory, fileWriteModel.Location);

        string fileName = Path.Combine(targetFolder, name);

        await File.AppendAllTextAsync(fileName, fileWriteModel.Text);

        return true;
    }
}
