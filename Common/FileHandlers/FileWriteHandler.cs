using Pirate.Common.FileHandlers.Interfaces;

namespace Pirate.Common.FileHandlers;


/// <summary>
/// This class handles writing to files.
/// </summary>
public class FileWriteHandler : BaseFileHandler, IFileWriteHandler
{
    public bool WriteToFile(FileWriteModel fileWriteModel, bool encryption = false)
    {
        if (fileWriteModel.Name == string.Empty || fileWriteModel.Extension == string.Empty || fileWriteModel.Extension == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        if (encryption) fileWriteModel.Text = Encrypt(fileWriteModel.Text);

        CreateFolder(fileWriteModel.Location);

        var result = Write(fileWriteModel).Result;

        return result;
    }
    public bool AppendToFile(FileWriteModel fileWriteModel, bool encryption = false)
    {
        if (fileWriteModel.Name == string.Empty || fileWriteModel.Text == string.Empty || fileWriteModel.Extension == string.Empty) { throw new ArgumentNullException("Name, Text or Extension provided is empty"); }

        if (encryption)
        {
            fileWriteModel.Text = Encrypt(fileWriteModel.Text);
        }

        CreateFolder(fileWriteModel.Location);

        var result = AppendWrite(fileWriteModel).Result;

        return result;
    }

    private string Encrypt(string text)
    {
        return text += " This should be encrypted.";
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
