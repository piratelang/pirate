using PirateLang.Commands.Models;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A command which initializes files based on the parameters.
/// </summary>
public class InitCommand : Command, ICommand, IInitCommand
{
    private IFileWriteHandler _fileWriteHandler;
    public InitCommand(ILogger Logger, IFileWriteHandler FileWriteHandler, IEnvironmentVariables EnvironmentVariables) : base(Logger, EnvironmentVariables)
    { 
        _fileWriteHandler = FileWriteHandler;
    }

    public override object Run(string[] arguments)
    {
        Logger.Info("Starting Init Command");
        var nameArgument = "main";
        if (arguments.Length == 2) { nameArgument = arguments[1]; }

        Logger.Info($"Creating {nameArgument} file");
        var fileName = nameArgument.Replace(".pirate", "");

        var text = String.Join(
            Environment.NewLine,
            "func main()",
            "{",
            "    print(\"Hello World\");",
            "}"
        );
        _fileWriteHandler.WriteToFile(new FileWriteModel(fileName, FileExtension.PIRATE, "", text));

        Logger.Info($"Created {nameArgument} file");
        Console.WriteLine($"\nCreated {fileName}.pirate");

        return true;
    }

    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate init command",
                usage: "pirate init [filename]",
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
