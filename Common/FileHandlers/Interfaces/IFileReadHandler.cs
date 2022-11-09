namespace Common.FileHandlers.Interfaces;

public interface IFileReadHandler
{
    ///<summary>
    /// Reads all text from a file
    ///</summary>
    ///<param name="fileReadModel">FileReadModel with all the information about the file</param>
    ///<returns>String with all the text from the file</returns>
    bool FileExists(string name, string extension, string location);

    ///<summary>
    /// Checks if a file exists
    ///</summary>
    ///<param name="name">Name of the file without extension</param>
    ///<param name="extension">Extension of the file with dot</param>
    ///<param name="location">Location of the file without root folder, i.e. without "./"</param>
    Task<string> ReadAllTextFromFile(string name, string extension, string location);

    ///<summary>
    /// Checks if a directory exists
    ///</summary>
    ///<param name="location">Location of the directory without root folder, i.e. "./"</param>
    bool DirectoryExists(string location);
}
