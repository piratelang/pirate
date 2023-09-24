using PirateLang.Commands.Interfaces;

namespace PirateLang.Commands;

public class NoCommand
{
    public static void Run(string version)
    {
        Console.WriteLine(string.Join(
            Environment.NewLine,
            GetASCIIArt(),
            $"PirateLang version {version}",
            "",
            "Commands:",
            " - pirate run [filename]",
            "    run the specified file",
            " - pirate init [filename]",
            "    initializes a new pirate project",
            " - pirate new [type]",
            "    create a new file",
            " - pirate build",
            "    build the modules in the current folder",
            " - pirate shell",
            "    opens the pirate repl"
        ));
    }

    private static string GetASCIIArt()
    {
        return @"
      ___                     ___           ___                       ___     
     /  /\      ___          /  /\         /  /\          ___        /  /\    
    /  /::\    /  /\        /  /::\       /  /::\        /  /\      /  /:/_   
   /  /:/\:\  /  /:/       /  /:/\:\     /  /:/\:\      /  /:/     /  /:/ /\  
  /  /:/~/:/ /__/::\      /  /:/~/:/    /  /:/~/::\    /  /:/     /  /:/ /:/_ 
 /__/:/ /:/  \__\/\:\__  /__/:/ /:/___ /__/:/ /:/\:\  /  /::\    /__/:/ /:/ /\
 \  \:\/:/      \  \:\/\ \  \:\/:::::/ \  \:\/:/__\/ /__/:/\:\   \  \:\/:/ /:/
  \  \::/        \__\::/  \  \::/~~~~   \  \::/      \__\/  \:\   \  \::/ /:/ 
   \  \:\        /__/:/    \  \:\        \  \:\           \  \:\   \  \:\/:/  
    \  \:\       \__\/      \  \:\        \  \:\           \__\/    \  \::/   
     \__\/                   \__\/         \__\/                     \__\/     
";
    }
}
