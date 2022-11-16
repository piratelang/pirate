using Newtonsoft.Json;

namespace Shell.ModuleList;

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

            _logger.Log($"Found Module {fileName}", "ModuleListRepository", LogType.INFO);
            moduleList.Add(new Module(fileName, filePath, lastModifiedDate));
        }
        string jsonString = JsonConvert.SerializeObject(moduleList, Formatting.Indented);
        _logger.Log($"Writing module list to \"{location}/modules.json\"", "ModuleListRepository", LogType.INFO);
        _fileWriteHandler.WriteToFile(new FileWriteModel("modules", FileExtension.JSON, location, jsonString));
    }

    public List<Module> GetList(string location)
    {
        if (!_fileReadHandler.FileExists("modules", FileExtension.JSON, location))
        {
            _logger.Log($"Creating module list at \"{location}/modules.json\"", "ModuleListRepository", LogType.INFO);
            _fileWriteHandler.WriteToFile(new FileWriteModel("modules", FileExtension.JSON, location, " "));
        }
        var file = _fileReadHandler.ReadAllTextFromFile("modules", FileExtension.JSON, location).Result;
        var deserialize = JsonConvert.DeserializeObject<List<Module>>(file);
        return deserialize;
    }
}