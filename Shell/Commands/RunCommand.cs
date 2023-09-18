using Pirate.Interpreter.Interfaces;
using PirateLang.Commands.Models;
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
        Logger.Info("Starting Run Command");
        var fileArgument = "main";
        if (arguments.Length >= 2) { fileArgument = arguments[1]; }
        var fileName = fileArgument.Replace(".pirate", "");

        if (!_fileReadHandler.FileExists(fileName, FileExtension.PIRATE, "")) Error($"File \"{fileArgument}\" not provided or does not exist.");

        Logger.Info("Starting build");
        BuildCommand.Run(arguments);
        Logger.Info("Completed Build");

        Logger.Info($"Executing {fileName}.pirate\n");

        //return Interpreter.StartInterpreter(fileName);
        return null;
    }

    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate run command",
                usage: "pirate run [filename]",
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
