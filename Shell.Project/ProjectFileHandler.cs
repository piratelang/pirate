using System.Xml.Serialization;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.FileHandler.Model;
using Shell.Project.Interfaces;
using Shell.Project.Models;

namespace Shell.Project;

public class ProjectFileHandler : IProjectFileHandler
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
        var projectFile = await _fileReadHandler.ReadAllTextFromFile(name, FileExtension.SHIP, path);

        if (projectFile == null) throw new Exception("Project file not found");


        XmlSerializer serializer = new(typeof(ProjectFile));
        var project = (ProjectFile)serializer.Deserialize(new StringReader(projectFile)) ?? throw new Exception("Exception while deserializing project file");

        return project;
    }

    public void WriteProjectFile(string path, ProjectFile project)
    {
        XmlSerializer serializer = new(typeof(ProjectFile));
        var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, project);

        _fileWriteHandler.WriteToFile(new FileWriteModel(
            fileName: "project",
            fileExtension: FileExtension.SHIP,
            fileLocation: path,
            fileText: stringWriter.ToString()
        ));
    }

}
