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

    public FileWriteModel(string fileName, string fileExtension, string fileLocation, string fileText)
    {
        Name = fileName;
        Extension = fileExtension;
        Location = fileLocation;
        Text = fileText;
    }
}