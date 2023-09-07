using Shell.ModuleList;
using Shell.Commands.Interfaces;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A command which builds a module.
/// </summary>
public class BuildCommand : Command, ICommand, IBuildCommand
{
    private IObjectSerializer _objectSerializer;
    private IParser _parser;
    private Lexer _lexer;
    private IModuleListRepository _moduleListRepository;
    private IFileReadHandler _fileReadHandler;
    private string Location;

    public BuildCommand(ILogger logger, IObjectSerializer objectSerializer, IParser parser, Lexer lexer, IModuleListRepository moduleListRepository, IFileReadHandler fileReadHandler, IEnvironmentVariables environmentVariables) : base(logger, environmentVariables)
    {
        _objectSerializer = objectSerializer;
        _parser = parser;
        _lexer = lexer;
        _moduleListRepository = moduleListRepository;
        _fileReadHandler = fileReadHandler;

        Location = environmentVariables.GetVariable("location");
    }
    
    public override object Run(string[] arguments)
    {
        Logger.Info("Starting Build Command");

        // Check for files
        var foundFiles = Directory.GetFiles("./", "*.pirate", SearchOption.AllDirectories);
        if (foundFiles.Length == 0) Error("No files were found in the directory");

        var moduleList = _moduleListRepository.GetList(Location);

        foreach (var file in foundFiles)
        {
            if (CheckModuleList(moduleList, file)) break;

            // Starting build
            Console.WriteLine($"Building {file}\n");
            Logger.Info($"Building {file}");

            var fileName = file.Replace(".pirate", "").Replace("./", "");
            
            var text = _fileReadHandler.ReadAllTextFromFile(fileName, FileExtension.PIRATE, "").Result;
            if (text == null) Error($"{fileName} contains no text");

            // Running Lexer
            Logger.Info($"Lexing {file}\n");
            var tokens = _lexer.MakeTokens(text, "test").ToList();
            if (tokens.Count() == 0) Error($"Error occured while lexing tokens, in the file {fileName}.");

            // Running Parser
            Logger.Info($"Parsing {file}\n");
            var parseResult = _parser.StartParse(tokens, fileName);
            if (parseResult.Nodes.Count() < 1) Error("Error occured while parsing tokens.");
        }
        Logger.Info($"Updating ModuleList\n");
        _moduleListRepository.SetList(foundFiles, Location);

        return true;
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
        if (moduleList == null) return false;
        var foundModule = moduleList.Where(a => a.moduleName == file.Replace("./", "")).FirstOrDefault();
        if (foundModule != null)
        {
            if (
                foundModule.moduleName == File.OpenRead(file).Name.Split("\\").Last() &&
                foundModule.path == File.OpenRead(file).Name &&
                foundModule.lastModifiedDate == File.GetLastWriteTimeUtc(file)
            )
            {
                Logger.Info($"{foundModule.moduleName} was not modified since last build");
                return true;
            }
        }
        return false;
    }
}
