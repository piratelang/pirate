using Newtonsoft.Json;

namespace Shell.ModuleList;

public class ModuleListRepository : IModuleListRepository
{
    private ILogger _logger;
    private IFileWriteHandler _fileWriteHandler;

    public ModuleListRepository(ILogger Logger, IFileWriteHandler FileWriteHandler)
    {
        _logger = Logger;
        _fileWriteHandler = FileWriteHandler;
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
        _fileWriteHandler.WriteToFile("modules", ".json", jsonString, location);
    }

    public List<Module> GetList(string location)
    {
        if (!File.Exists($"{location}/modules.json"))
        {
            _logger.Log($"Creating module list at \"{location}/modules.json\"", "ModuleListRepository", LogType.INFO);
            _fileWriteHandler.WriteToFile("modules", ".json", "", location);
        }
        var file = File.ReadAllText($"{location}/modules.json");
        var deserialize = JsonConvert.DeserializeObject<List<Module>>(file);
        return deserialize;
    }
}