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

    public override void Run(string[] arguments)
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
    }

    public override void Help()
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            "Description",
            "   pirate initalize command",
            "\nUsage",
            "   pirate init [filename]",
            "\nOptions",
            "   -h --help       Show command line help."
        ));
    }
}
