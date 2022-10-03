using Common;
using Common.Enum;
using Newtonsoft.Json;
using PirateLexer;
using PirateParser;
using Shell.Commands.ModuleView;

namespace Shell.Commands
{
    public class BuildCommand : ICommand
    {
        public string version { get; set; }
        public BuildCommand(string Version)
        {   
            version = Version;
        }
        public void Run(string[] arguments)
        {
            Logger.Log("Starting Build Command", this.GetType().Name, LogType.INFO);
            string[] foundFiles = Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories);
            var location = $"bin/pirate{version}";

            if (foundFiles.Length == 0)
            {
                Logger.Log("No files were found in the directory", this.GetType().Name, LogType.ERROR);
                Error("No files found");
                return;
            }
            var moduleList = GetModules(foundFiles, location);

            foreach (var file in foundFiles)
            {
                // Module foundModule = moduleList.Where(a => a.moduleName == file.Replace("./", "")).FirstOrDefault();
                // if (
                //     foundModule.moduleName == File.OpenRead(file).Name.Split("\\").Last() &&
                //     foundModule.path == File.OpenRead(file).Name &&
                //     foundModule.lastModifiedDate == File.GetLastWriteTimeUtc(file)
                // )
                // {
                //     Logger.Log($"{foundModule.moduleName} was not modified since last build");
                //     break;
                // }

                Console.WriteLine($"Building {file}");
                Logger.Log($"Building {file}", this.GetType().Name, LogType.INFO);
                var fileName = file.Replace(".pirate", "");
                var text = File.ReadAllText(fileName + ".pirate");
                if(text == null)
                {
                    Logger.Log($"{fileName} contains no text", this.GetType().Name, LogType.ERROR);
                    Error($"{fileName} contains no text");
                    return;
                }

                Logger.Log($"Lexing {file}", this.GetType().Name, LogType.INFO);
                var lexer = new Lexer("test", text);
                var tokens = lexer.MakeTokens();
                if (tokens.tokens.Count() == 0)
                {
                    Logger.Log($"Error occured while lexing tokens, in the file {fileName}. {tokens.error.AsString()}", this.GetType().Name, LogType.ERROR);
                    Error($"Error occured while lexing tokens, in the file {fileName}\n");
                    return;
                }

                Logger.Log($"Parsing {file}", this.GetType().Name, LogType.INFO);
                var parser = new Parser(tokens.tokens);
                var parseResult = parser.Parse(location, fileName);
                if (parseResult != true)
                {
                    Logger.Log("Error occured while parsing tokens.", this.GetType().Name, LogType.ERROR);
                    Error("Error occured while parsing tokens.");
                    return;
                }
            }
        }
        public void Help()
        {
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "Description",
                "   pirate project building command",
                "\nUsage",
                "   pirate build",
                "\nOptions",
                "   -h --help       Show command line help."
            ));
        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public List<Module> GetModules(string[] foundFiles, string location)
        {
            List<Module> moduleList = new() { };

            foreach (var item in foundFiles)
            {
                var file = File.OpenRead(item);
                var filePath = file.Name;

                var name = filePath.Split("\\");
                var fileName = name.Last();

                var lastModifiedDate = File.GetLastWriteTimeUtc(item);

                Logger.Log($"Found Module {fileName}", this.GetType().Name, LogType.INFO);
                moduleList.Add(new Module(fileName, filePath, lastModifiedDate));
            }
            string jsonString = JsonConvert.SerializeObject(moduleList);
            Logger.Log($"Writing module list to {location}/modules.json", this.GetType().Name, LogType.INFO);
            File.WriteAllTextAsync($"{location}/modules.json", jsonString);

            return moduleList;
        }
    }
}