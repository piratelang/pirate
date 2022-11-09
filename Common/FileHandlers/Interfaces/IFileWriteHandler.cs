namespace Common.FileHandlers.Interfaces;

public interface IFileWriteHandler
{
    ///<summary>
    /// Writes all text to a file
    ///</summary>
    ///<param name="fileWriteModel">FileWriteModel with all the information about the file</param>
    bool AppendToFile(FileWriteModel fileWriteModel, bool encryption = false);

    ///<summary>
    /// Writes all text to a file
    ///</summary>
    ///<param name="fileWriteModel">FileWriteModel with all the information about the file</param>
    bool WriteToFile(FileWriteModel fileWriteModel, bool encryption = false);
}
