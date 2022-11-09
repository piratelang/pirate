using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.FileHandlers;

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
    public FileWriteModel(string fileName, string fileExtension, string fileLocation, string fileText)
    {
        Name = fileName;
        Extension = fileExtension;
        Location = fileLocation;
        Text = fileText;
    }
}