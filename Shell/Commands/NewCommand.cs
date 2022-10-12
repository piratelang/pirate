using Common;
using Common.Enum;

namespace Shell.Commands
{
    public class NewCommand : Command
    {
        public string version { get; set; }
        public Logger logger { get; set; }
        public NewCommand(string Version, Logger Logger)
        {
            version = Version;
            logger = Logger;
        }
        public override void Run(string[] arguments)
        {
            logger.Log("Starting New Command", this.GetType().Name, LogType.INFO);
            var typeArgument = string.Empty;
            if (arguments.Length >= 2) { typeArgument = arguments[1]; }

            if (typeArgument == string.Empty)
            {
                logger.Log("Argument is empty", this.GetType().Name, LogType.INFO);
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
                logger.Log($"Specified file \"{typeArgument}\" not able to be created", this.GetType().Name, LogType.ERROR);
                Error($"Specified file \"{typeArgument}\" not able to be created");
                return;
            }

            logger.Log($"Creating {typeArgument} file", this.GetType().Name, LogType.INFO);
            switch (typeArgument)
            {
                case "gitignore":
                    var file = File.CreateText($"./.gitignore");
                    file.Write("[Bb]in/");
                    file.Close();
                    return;
                case "gitattributes":
                    file = File.CreateText("./.gitattributes");
                    file.Write("*.pirate linguist-language=Squirrel");
                    file.Close();
                    return;
                case "pirate":
                    var filename = "main";
                    try
                    {
                        filename = arguments[2];
                    }
                    catch (System.Exception){}
                    var exists = File.Exists($"./{filename}.pirate");
                    if (exists)
                    {
                        logger.Log($"Specified filename \"{filename}\" already exists", this.GetType().Name, LogType.WARNING);
                        Error($"Specified filename \"{filename}\" already exists");
                        return;
                    }
                    file = File.CreateText($"./{filename}.pirate");
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
}