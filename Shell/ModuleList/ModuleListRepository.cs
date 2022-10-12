using Common;
using Common.Enum;
using Newtonsoft.Json;

namespace Shell.ModuleList
{
    public class ModuleListRepository
    {
        public static void SetList(string[] foundFiles, string location, Logger logger)
        {
            List<Module> moduleList = new() { };

            foreach (var item in foundFiles)
            {
                var file = File.OpenRead(item);
                var filePath = file.Name;

                var name = filePath.Split("\\");
                var fileName = name.Last();

                var lastModifiedDate = File.GetLastWriteTimeUtc(item);

                logger.Log($"Found Module {fileName}", "ModuleListRepository", LogType.INFO);
                moduleList.Add(new Module(fileName, filePath, lastModifiedDate));
            }
            string jsonString = JsonConvert.SerializeObject(moduleList, Formatting.Indented);
            logger.Log($"Writing module list to \"{location}/modules.json\"", "ModuleListRepository", LogType.INFO);
            File.WriteAllTextAsync($"{location}/modules.json", jsonString);
        }

        public static List<Module> GetList(string location, Logger logger)
        {
            if(!File.Exists($"{location}/modules.json"))
            {
                logger.Log($"Creating module list at \"{location}/modules.json\"", "ModuleListRepository", LogType.INFO);
                var createdFile = File.Create($"{location}/modules.json");
                createdFile.Close();
            }
            var file = File.ReadAllText($"{location}/modules.json");
            var deserialize = JsonConvert.DeserializeObject<List<Module>>(file);
            return deserialize;
        }
    }
}