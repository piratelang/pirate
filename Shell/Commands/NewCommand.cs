using PirateLang.Commands.Models;
using Shell.Commands.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A command which creates a new file.
/// </summary>
public class NewCommand : Command, ICommand, INewCommand
{
    private IFileWriteHandler _fileWriteHandler;
    private IFileReadHandler _fileReadHandler;
    public NewCommand(ILogger Logger, IFileWriteHandler FileWriteHandler, IFileReadHandler FileReadHandler, IEnvironmentVariables EnvironmentVariables) : base(Logger, EnvironmentVariables)
    { 
        _fileWriteHandler = FileWriteHandler;
        _fileReadHandler = FileReadHandler;
    }
    public override object Run(string[] arguments)
    {
        Logger.Log("Starting New Command", LogType.INFO);
        var typeArgument = string.Empty;
        if (arguments.Length >= 2) { typeArgument = arguments[1]; }

        if (typeArgument == string.Empty)
        {
            Logger.Log("Argument is empty", LogType.INFO);
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "\nThe \"pirate new [type]\" command creates a new file from a template",
                "\nOptions",
                " - gitignore",
                " - gitattributes",
                " - pirate"
            ));
            return true;
        }

        var typeOptions = new string[]{
                "gitignore",
                "gitattributes",
                "pirate"
            };
        if (!typeOptions.Contains(typeArgument))
        {
            Logger.Log($"Specified file \"{typeArgument}\" not able to be created", LogType.ERROR);
            Error($"Specified file \"{typeArgument}\" not able to be created");
            return true;
        }

        Logger.Log($"Creating {typeArgument} file", LogType.INFO);
        switch (typeArgument)
        {
            case "gitignore":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitignore,  "", "[Bb]in/"));
                return true;
            case "gitattributes":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitattributes, "", "*.pirate linguist-language=Squirrel" ));
                return true;
            case "pirate":
                var filename = "main";
                try
                {
                    filename = arguments[2];
                }
                catch (System.Exception) { }

                if (_fileReadHandler.FileExists(filename, FileExtension.PIRATE, " "))
                {
                    Error($"Specified filename \"{filename}\" already exists");
                    return true;
                }
                _fileWriteHandler.WriteToFile(new FileWriteModel(filename, FileExtension.PIRATE, " ", ""));
                Console.WriteLine($"\nCreated new .{typeArgument} file");
                return true;
        }
        return true;
    }

    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate new command",
                usage: "pirate new [type] [filename]",
                options: new List<OptionDescription>()
                {
                    new OptionDescription(
                        options: new List<string>() { "pirate" },
                        description: "Creates new pirate module"
                    ),
                    new OptionDescription(
                        options: new List<string>() { "gitignore" },
                        description: "Creates standard pirate gitginore"
                    ),
                    new OptionDescription(
                        options: new List<string>() { "gitattributes" },
                        description: "Creates standard pirate gitattributes"
                    ),
                    new OptionDescription(
                        options: new List<string>() { "-h", "--help" },
                        description: "Show command line help."
                    )
                }
            ).ToString()
        );

    }
}
