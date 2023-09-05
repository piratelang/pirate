using Pirate.Interpreter.Interfaces;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A command which builds and then runs the code.
/// </summary>
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
    public override object Run(string[] arguments)
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

        return Interpreter.StartInterpreter(fileName);
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
