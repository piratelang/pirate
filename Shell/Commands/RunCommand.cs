using Common;
using PirateInterpreter.Interfaces;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

public class RunCommand : Command, ICommand, IRunCommand
{
    public IObjectSerializer ObjectSerializer;
    public IBuildCommand BuildCommand;
    public IInterpreter Interpreter { get; set; }
    private IFileReadHandler _fileReadHandler;
    public string Location;
    public RunCommand(ILogger Logger, IObjectSerializer objectSerializer, IBuildCommand buildCommand, IInterpreter interpreter, IFileReadHandler FileReadHandler, IEnvironmentVariables EnvironmentVariables) : base(Logger, EnvironmentVariables)
    {
        ObjectSerializer = objectSerializer;
        BuildCommand = buildCommand;
        Interpreter = interpreter;
        _fileReadHandler = FileReadHandler;
        Location = EnvironmentVariables.GetVariable("location");
    }
    public override void Run(string[] arguments)
    {
        Logger.Log("Starting Run Command", LogType.INFO);
        var fileArgument = "main";
        if (arguments.Length >= 2) { fileArgument = arguments[1]; }
        var fileName = fileArgument.Replace(".pirate", "");

        if (!_fileReadHandler.FileExists(fileName, FileExtension.PIRATE, "")) Error($"File \"{fileArgument}\" not provided or does not exist.");

        Logger.Log("Starting build", LogType.INFO);
        BuildCommand.Run(arguments);
        Logger.Log("Completed Build", LogType.INFO);

        Logger.Log($"Executing {fileName}.pirate\n", LogType.INFO);

        var interpreterResult = Interpreter.StartInterpreter(fileName);
        // foreach (var item in interpreterResult)
        // {
        //     if (item == null)
        //     {
        //         Logger.Log("Interpreter returned null", LogType.ERROR);
        //         continue;
        //     }

        //     if (item.Value is not null)
        //     {
        //         Console.WriteLine(item.Value.ToString());
        //     }
        //     else
        //     {
        //         Console.WriteLine("item is null");
        //     }
        // }
    }

    public override void Help()
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            "Description",
            "   pirate run command",
            "\nUsage",
            "   pirate run [filename]",
            "\nOptions",
            "   -h --help   Show command line help."
        ));
    }
}
