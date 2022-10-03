using Common;
using Common.Enum;
using PirateLexer;
using PirateParser;
using Shell.ModuleList;

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
            List<Module> moduleList = ModuleListRepository.GetList(location);

            foreach (var file in foundFiles)
            {
                if (moduleList.Count > 0)
                {
                    Module foundModule = moduleList.Where(a => a.moduleName == file.Replace("./", "")).FirstOrDefault();
                    if (foundModule != null)
                    {
                        if (
                            foundModule.moduleName == File.OpenRead(file).Name.Split("\\").Last() &&
                            foundModule.path == File.OpenRead(file).Name &&
                            foundModule.lastModifiedDate == File.GetLastWriteTimeUtc(file)
                        )
                        {
                            Logger.Log($"{foundModule.moduleName} was not modified since last build", this.GetType().Name, LogType.INFO);
                            break;
                        }
                    }
                    
                }
                
                Console.WriteLine($"Building {file}\n");
                Logger.Log($"Building {file}", this.GetType().Name, LogType.INFO);
                var fileName = file.Replace(".pirate", "");
                var text = File.ReadAllText(fileName + ".pirate");
                if(text == null)
                {
                    Logger.Log($"{fileName} contains no text", this.GetType().Name, LogType.ERROR);
                    Error($"{fileName} contains no text");
                    return;
                }

                Logger.Log($"Lexing {file}\n", this.GetType().Name, LogType.INFO);
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
            ModuleListRepository.SetList(foundFiles, location);
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
    }
}