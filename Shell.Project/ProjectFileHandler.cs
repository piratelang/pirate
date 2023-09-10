using System.Xml.Serialization;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Shell.Project.Models;

namespace Shell.Project;

public class ProjectFileHandler
{
    private readonly IFileReadHandler _fileReadHandler;
    private readonly IFileWriteHandler _fileWriteHandler;

    public ProjectFileHandler(IFileReadHandler fileReadHandler, IFileWriteHandler fileWriteHandler)
    {
        _fileReadHandler = fileReadHandler;
        _fileWriteHandler = fileWriteHandler;
    }

    public async Task<ProjectFile?> ReadProjectFile(string name, string path)
    {
        var projectFile = await _fileReadHandler.ReadAllTextFromFile(name, FileExtension.PIRATEPROJ, path);

        if (projectFile == null) throw new Exception("Project file not found");
        

        XmlSerializer serializer = new(typeof(ProjectFile));
        var project = (ProjectFile)serializer.Deserialize(new StringReader(projectFile));

        return project;
    }

    public async Task WriteProjectFile(string path, ProjectFile project)
    {
        XmlSerializer serializer = new(typeof(ProjectFile));
        var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, project);

        _fileWriteHandler.WriteToFile(new Pirate.Common.FileHandler.Model.FileWriteModel(
            fileName: "project",
            fileExtension: FileExtension.PIRATEPROJ,
            fileLocation: path,
            fileText: stringWriter.ToString()
        ));
    }

}
