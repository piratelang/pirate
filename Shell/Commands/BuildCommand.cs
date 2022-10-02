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
            string[] foundFiles = Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories);
            var location = $"bin/pirate{version}";

            if (foundFiles.Length == 0)
            {
                Error("No files found");
                return;
            }
            var moduleList = GetModules(foundFiles, location);

            foreach (var file in foundFiles)
            {
                Module foundModule = moduleList.Where(a => a.moduleName == file.Replace("./", "")).FirstOrDefault();
                if (
                    foundModule.moduleName == File.OpenRead(file).Name.Split("\\").Last() &&
                    foundModule.path == File.OpenRead(file).Name &&
                    foundModule.lastModifiedDate == File.GetLastWriteTimeUtc(file)
                )
                {
                    break;
                }

                Console.WriteLine($"Building {file}");
                var fileName = file.Replace(".pirate", "");
                var text = File.ReadAllText(fileName + ".pirate");
                if(text == null)
                {
                    Error($"{fileName} contains no text");
                    return;
                }

                var lexer = new Lexer("test", text);
                var tokens = lexer.MakeTokens();
                if (tokens.tokens.Count() == 0)
                {
                    Error($"Error occured while lexing tokens, in the file {fileName}\n");
                    return;
                }

                var parser = new Parser(tokens.tokens);
                var parseResult = parser.Parse(location, fileName);
                if (parseResult != true)
                {
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

                moduleList.Add(new Module(fileName, filePath, lastModifiedDate));
            }
            string jsonString = JsonConvert.SerializeObject(moduleList);
            File.WriteAllTextAsync($"{location}/modules.json", jsonString);

            return moduleList;
        }
    }
}