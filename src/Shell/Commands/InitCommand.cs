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
        Logger.Log("Starting Init Command", LogType.INFO);
        var nameArgument = "main";
        if (arguments.Length == 2) { nameArgument = arguments[1]; }

        Logger.Log($"Creating {nameArgument} file", LogType.INFO);
        var fileName = nameArgument.Replace(".pirate", "");

        var text = String.Join(
            Environment.NewLine,
            "func main()",
            "{",
            "    print(\"Hello World\");",
            "}"
        );
        _fileWriteHandler.WriteToFile(new FileWriteModel(fileName, FileExtension.PIRATE, "", text));

        Logger.Log($"Created {nameArgument} file", LogType.INFO);
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
