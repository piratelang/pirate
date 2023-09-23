using Shell.ModuleList;
using Shell.Commands.Interfaces;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;
using PirateLang.Commands.Models;

namespace Shell.Commands;

/// <summary>
/// A command which builds a module.
/// </summary>
public class BuildCommand : Command, ICommand, IBuildCommand
{
    private IParser _parser;
    private Lexer _lexer;
    private IFileReadHandler _fileReadHandler;
    private IProjectFileHandler _projectFileHandler;
    private string Location;

    public BuildCommand(ILogger logger, IParser parser, Lexer lexer, IFileReadHandler fileReadHandler, IEnvironmentVariables environmentVariables, IProjectFileHandler projectFileHandler) : base(logger, environmentVariables)
    {
        _parser = parser;
        _lexer = lexer;
        _fileReadHandler = fileReadHandler;
        _projectFileHandler = projectFileHandler;

        Location = environmentVariables.GetVariable("location");
    }
    
    public override object Run(string[] arguments)
    {
        Logger.Info("Starting Build Command");

        // check for files
        var foundfiles = directory.getfiles("./", "*.ship", searchoption.alldirectories);
        if (foundfiles.length == 0) error("no project files were found in the directory");

         
        
        return true;
    }
    
    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate build command",
                usage: "pirate build",
                options: new List<OptionDescription>()
                {
                    new OptionDescription(
                        options: new List<string>() { "-h", "--help" },
                        description: "Show command line help."
                    )
                }

            ).ToString()
        );
    }
}
