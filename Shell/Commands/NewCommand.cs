using System.Security.AccessControl;

using PirateInterpreter;
using PirateLexer;
using PirateParser;

namespace Shell.Commands
{
    public class NewCommand : ICommand
    {
        public string version { get; set; }
        public NewCommand(string Version)
        {
            version = Version;
        }
        public void Run(string[] arguments)
        {
            var typeArgument = string.Empty;
            if (arguments.Length >= 2) { typeArgument = arguments[1]; }

            if (typeArgument == string.Empty)
            {
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
                Error($"Specified file \"{typeArgument}\" not able to be created");
                return;
            }
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
                        Error($"Specified filename \"{filename}\" already exists");
                        return;
                    }
                    file = File.CreateText($"./{filename}.pirate");
                    Console.WriteLine($"\nCreated new .{typeArgument} file");
                    return;
            }      
        }

        public void Help()
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

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}