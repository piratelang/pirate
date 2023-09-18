using Newtonsoft.Json;
using Shell.ModuleList.interfaces;

namespace Shell.ModuleList;

/// <summary>
/// A repository handling the module list file.
/// </summary>
public class ModuleListRepository : IModuleListRepository
{
    private ILogger _logger;
    private IFileWriteHandler _fileWriteHandler;
    private IFileReadHandler _fileReadHandler;

    public ModuleListRepository(ILogger Logger, IFileWriteHandler FileWriteHandler, IFileReadHandler FileReadHandler)
    {
        _logger = Logger;
        _fileWriteHandler = FileWriteHandler;
        _fileReadHandler = FileReadHandler;
    }
    public void SetList(string[] foundFiles, string location)
    {
        List<Module> moduleList = new() { };

        foreach (var item in foundFiles)
        {
            var file = File.OpenRead(item);
            var filePath = file.Name;

            var name = filePath.Split("\\");
            var fileName = name.Last();

            var lastModifiedDate = File.GetLastWriteTimeUtc(item);

            _logger.Info($"Found Module {fileName}");
            moduleList.Add(new Module(fileName, filePath, lastModifiedDate));
        }
        string jsonString = JsonConvert.SerializeObject(moduleList, Formatting.Indented);
        _logger.Info($"Writing module list to \"{location}/modules.json\"");
        _fileWriteHandler.WriteToFile(new FileWriteModel("modules", FileExtension.JSON, location, jsonString));
    }

    public List<Module> GetList(string location)
    {
        if (!_fileReadHandler.FileExists("modules", FileExtension.JSON, location))
        {
            _logger.Info($"Creating module list at \"{location}/modules.json\"");
            _fileWriteHandler.WriteToFile(new FileWriteModel("modules", FileExtension.JSON, location, " "));
        }
        var file = _fileReadHandler.ReadAllTextFromFile("modules", FileExtension.JSON, location).Result;
        var deserialize = JsonConvert.DeserializeObject<List<Module>>(file);
        return deserialize;
    }
}