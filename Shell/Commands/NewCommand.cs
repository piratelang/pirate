using Shell.Commands.Interfaces;

namespace Shell.Commands;

public class NewCommand : Command, ICommand, INewCommand
{
    private IFileWriteHandler _fileWriteHandler;
    private IFileReadHandler _fileReadHandler;
    public NewCommand(ILogger Logger, IFileWriteHandler FileWriteHandler, IFileReadHandler FileReadHandler, IEnvironmentVariables EnvironmentVariables) : base(Logger, EnvironmentVariables)
    { 
        _fileWriteHandler = FileWriteHandler;
        _fileReadHandler = FileReadHandler;
    }
    public override void Run(string[] arguments)
    {
        Logger.Log("Starting New Command", this.GetType().Name, LogType.INFO);
        var typeArgument = string.Empty;
        if (arguments.Length >= 2) { typeArgument = arguments[1]; }

        if (typeArgument == string.Empty)
        {
            Logger.Log("Argument is empty", this.GetType().Name, LogType.INFO);
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "\nThe \"pirate new [type]\" command creates a new file from a template",
                "\nOptions",
                " - gitignore",
                " - gitattributes",
                " - pirate"
            ));
            return;
        }

        var typeOptions = new string[]{
                "gitignore",
                "gitattributes",
                "pirate"
            };
        if (!typeOptions.Contains(typeArgument))
        {
            Logger.Log($"Specified file \"{typeArgument}\" not able to be created", this.GetType().Name, LogType.ERROR);
            Error($"Specified file \"{typeArgument}\" not able to be created");
            return;
        }

        Logger.Log($"Creating {typeArgument} file", this.GetType().Name, LogType.INFO);
        switch (typeArgument)
        {
            case "gitignore":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitignore,  "", "[Bb]in/"));
                return;
            case "gitattributes":
                _fileWriteHandler.WriteToFile(new FileWriteModel("", FileExtension.gitattributes, "", "*.pirate linguist-language=Squirrel" ));
                return;
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
                    return;
                }
                _fileWriteHandler.WriteToFile(new FileWriteModel(filename, FileExtension.PIRATE, " ", ""));
                Console.WriteLine($"\nCreated new .{typeArgument} file");
                return;
        }
    }

    public override void Help()
    {
        Console.WriteLine(String.Join(
            Environment.NewLine,
            "Description",
            "   pirate new command",
            "\nUsage",
            "   pirate new [type] [filename]",
            "\nOptions",
            "   pirate          Creates new pirate module",
            "   gitignore       Creates standard pirate gitginore",
            "   gitattributes   Creates standard pirate gitattributes\n",
            "   -h --help   Show command line help."
        ));
    }
}
