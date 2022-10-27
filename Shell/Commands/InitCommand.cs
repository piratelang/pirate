using Common.Enum;
using Common.Interfaces;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

public class InitCommand : Command, ICommand, IInitCommand
{
    public InitCommand(ILogger Logger) : base(Logger)
    { }

    public override void Run(string[] arguments)
    {
        Logger.Log("Starting Init Command", this.GetType().Name, LogType.INFO);
        var nameArgument = "main";
        if (arguments.Length == 2) { nameArgument = arguments[1]; }

        Logger.Log($"Creating {nameArgument} file", this.GetType().Name, LogType.INFO);
        var fileName = nameArgument.Replace(".pirate", "");
        var file = File.CreateText($"./{fileName}.pirate");
        file.Write(String.Join(
            Environment.NewLine,
            "func main()",
            "{",
            "    print(\"Hello World\");",
            "}"
        ));
        file.Close();
        Logger.Log($"Created {nameArgument} file", this.GetType().Name, LogType.INFO);
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
