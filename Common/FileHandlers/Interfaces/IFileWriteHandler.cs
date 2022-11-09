namespace Common.FileHandlers.Interfaces;

public interface IFileWriteHandler
{
    bool AppendToFile(FileWriteModel fileWriteModel, bool encryption = false);
    bool WriteToFile(FileWriteModel fileWriteModel, bool encryption = false);
}
