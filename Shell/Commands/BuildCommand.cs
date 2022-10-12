using Common;
using Common.Enum;
using Lexer;
using Parser;
using Shell.ModuleList;

namespace Shell.Commands
{
    public class BuildCommand : Command
    {
        public string version { get; set; }
        public Logger logger { get; set; }
        public ObjectSerializer ObjectSerializer { get; set;}
        public string Location { get; set; }
        public BuildCommand(string Version, Logger Logger, ObjectSerializer objectSerializer, string location)
        {   
            version = Version;
            logger = Logger;
            ObjectSerializer = objectSerializer;
            Location = location;
        }
        public List<Scope> Run(string[] arguments)
        {
            logger.Log("Starting Build Command", this.GetType().Name, LogType.INFO);
            string[] foundFiles = Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories);
            

            if (foundFiles.Length == 0)
            {
                logger.Log("No files were found in the directory", this.GetType().Name, LogType.ERROR);
                Error("No files found");
                return null;
            }
            List<Module> moduleList = ModuleListRepository.GetList(Location, logger);
            List<Scope> scopeList = new();

            foreach (var file in foundFiles)
            {
                if (moduleList != null)
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
                            logger.Log($"{foundModule.moduleName} was not modified since last build", this.GetType().Name, LogType.INFO);
                            break;
                        }
                    }
                    
                }
                
                Console.WriteLine($"Building {file}\n");
                logger.Log($"Building {file}", this.GetType().Name, LogType.INFO);
                var fileName = file.Replace(".pirate", "");
                var text = File.ReadAllText(fileName + ".pirate");
                if(text == null)
                {
                    logger.Log($"{fileName} contains no text", this.GetType().Name, LogType.ERROR);
                    Error($"{fileName} contains no text");
                    return null;
                }

                logger.Log($"Lexing {file}\n", this.GetType().Name, LogType.INFO);
                var lexer = new Lexer.Lexer("test", text, logger);
                var tokens = lexer.MakeTokens();
                if (tokens.tokens.Count() == 0)
                {
                    logger.Log($"Error occured while lexing tokens, in the file {fileName}. {tokens.error.AsString()}", this.GetType().Name, LogType.ERROR);
                    Error($"Error occured while lexing tokens, in the file {fileName}\n");
                    return null;
                }

                logger.Log($"Parsing {file}\n", this.GetType().Name, LogType.INFO);
                var parser = new Parser.Parser(tokens.tokens, logger, ObjectSerializer, fileName);
                var parseResult = parser.StartParse();
                if (parseResult.Nodes.Count() < 1)
                {
                    logger.Log("Error occured while parsing tokens.", this.GetType().Name, LogType.ERROR);
                    Error("Error occured while parsing tokens.");
                    return null;
                }
                scopeList.Add(parseResult);
            }

            logger.Log($"Updating ModuleList\n", this.GetType().Name, LogType.INFO);
            ModuleListRepository.SetList(foundFiles, Location, logger);

            return scopeList;
        }
        public override void Help()
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
    }
}