using Common;
using Common.Enum;
using Lexer;
using Parser;
using Shell.ModuleList;
using Shell.Commands.Interfaces;
using Common.Interfaces;

namespace Shell.Commands;

public class BuildCommand : Command, ICommand, IBuildCommand
{
    private IObjectSerializer ObjectSerializer;
    private IParser Parser;
    private ILexer Lexer;
    private string Location = EnvironmentVariables.GetVariable("location");

    public BuildCommand(ILogger logger, IObjectSerializer objectSerializer, IParser parser, ILexer lexer) : base(logger)
    {
        ObjectSerializer = objectSerializer;
        Parser = parser;
        Lexer = lexer;
    }
    public override void Run(string[] arguments)
    {
        Logger.Log("Starting Build Command", this.GetType().Name, LogType.INFO);

        // Check for files
        var foundFiles = Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories);
        if (foundFiles.Length == 0) Error("No files were found in the directory");

        var moduleList = ModuleListRepository.GetList(Location, Logger);
        List<Scope> scopeList = new();


        foreach (var file in foundFiles)
        {
            if (CheckModuleList(moduleList, file)) break;

            // Starting build
            Console.WriteLine($"Building {file}\n");
            Logger.Log($"Building {file}", this.GetType().Name, LogType.INFO);

            var fileName = file.Replace(".pirate", "");
            var text = File.ReadAllText(fileName + ".pirate");
            if (text == null) Error($"{fileName} contains no text");

            // Running Lexer
            Logger.Log($"Lexing {file}\n", this.GetType().Name, LogType.INFO);
            var tokens = Lexer.MakeTokens(text, "test");
            if (tokens.tokens.Count() == 0) Error($"Error occured while lexing tokens, in the file {fileName}. {tokens.error.AsString()}");

            // Running Parser
            Logger.Log($"Parsing {file}\n", this.GetType().Name, LogType.INFO);
            var parseResult = Parser.StartParse(tokens.tokens, fileName);
            if (parseResult.Nodes.Count() < 1) Error("Error occured while parsing tokens.");

            scopeList.Add(parseResult);
        }

        Logger.Log($"Updating ModuleList\n", this.GetType().Name, LogType.INFO);
        ModuleListRepository.SetList(foundFiles, Location, Logger);
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

    public bool CheckModuleList(List<Module> moduleList, string file)
    {
        if (moduleList != null) return false;
        var foundModule = moduleList.Where(a => a.moduleName == file.Replace("./", "")).FirstOrDefault();
        if (foundModule != null)
        {
            if (
                foundModule.moduleName == File.OpenRead(file).Name.Split("\\").Last() &&
                foundModule.path == File.OpenRead(file).Name &&
                foundModule.lastModifiedDate == File.GetLastWriteTimeUtc(file)
            )
            {
                Logger.Log($"{foundModule.moduleName} was not modified since last build", this.GetType().Name, LogType.INFO);
                return true;
            }
        }
        return false;
    }
}
