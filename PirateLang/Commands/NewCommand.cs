using PirateLang.Commands.Interfaces;
using PirateLang.Commands.Models;

namespace PirateLang.Commands;

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
        Logger.Info("Starting New Command");
        var typeArgument = string.Empty;
        if (arguments.Length >= 2) { typeArgument = arguments[1]; }

        if (typeArgument == string.Empty)
        {
            Logger.Info("Argument is empty");
            Console.WriteLine(string.Join(
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
            Logger.Error($"Specified file \"{typeArgument}\" not able to be created");
            Error($"Specified file \"{typeArgument}\" not able to be created");
            return true;
        }

        Logger.Info($"Creating {typeArgument} file");
        switch (typeArgument)
        {
            case "gitignore":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitignore, "", "[Bb]in/"));
                return true;
            case "gitattributes":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitattributes, "", "*.pirate linguist-language=Squirrel"));
                return true;
            case "pirate":
                var filename = "main";
                try
                {
                    filename = arguments[2];
                }
                catch (Exception) { }

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
