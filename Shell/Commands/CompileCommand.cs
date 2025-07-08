using Shell.ModuleList;
using Shell.Commands.Interfaces;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;
using PirateLang.Commands.Models;
using Pirate.Compiler.Interfaces;

namespace Shell.Commands;

/// <summary>
/// A command which compiles pirate code to executable output
/// </summary>
public class CompileCommand : Command, ICommand, ICompileCommand
{
    private IObjectSerializer _objectSerializer;
    private IParser _parser;
    private Lexer _lexer;
    private IModuleListRepository _moduleListRepository;
    private IFileReadHandler _fileReadHandler;
    private ICompiler _compiler;
    private string Location;

    public CompileCommand(ILogger logger, IObjectSerializer objectSerializer, IParser parser, Lexer lexer, IModuleListRepository moduleListRepository, IFileReadHandler fileReadHandler, ICompiler compiler, IEnvironmentVariables environmentVariables) : base(logger, environmentVariables)
    {
        _objectSerializer = objectSerializer;
        _parser = parser;
        _lexer = lexer;
        _moduleListRepository = moduleListRepository;
        _fileReadHandler = fileReadHandler;
        _compiler = compiler;
        Location = environmentVariables.GetVariable("location");
    }
    
    public override object Run(string[] arguments)
    {
        Logger.Info("Starting Compile Command");

        var fileArgument = "main";
        var outputPath = "./output";
        
        // Parse arguments
        for (int i = 1; i < arguments.Length; i++)
        {
            switch (arguments[i])
            {
                case "-o":
                case "--output":
                    if (i + 1 < arguments.Length)
                    {
                        outputPath = arguments[i + 1];
                        i++; // Skip next argument
                    }
                    break;
                default:
                    if (!arguments[i].StartsWith("-"))
                    {
                        fileArgument = arguments[i];
                    }
                    break;
            }
        }

        var fileName = fileArgument.Replace(".pirate", "");
        
        if (!_fileReadHandler.FileExists(fileName, FileExtension.PIRATE, ""))
        {
            Error($"File \"{fileArgument}\" not provided or does not exist.");
        }

        // Create output directory if it doesn't exist
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        try
        {
            Logger.Info($"Compiling {fileName}.pirate");
            
            // Read the pirate file
            var text = _fileReadHandler.ReadAllTextFromFile(fileName, FileExtension.PIRATE, "").Result;
            if (text == null) Error($"{fileName} contains no text");

            // Running Lexer
            Logger.Info($"Lexing {fileName}.pirate");
            var tokens = _lexer.MakeTokens(text, "test").ToList();
            if (tokens.Count() == 0) Error($"Error occurred while lexing tokens, in the file {fileName}.");

            // Running Parser
            Logger.Info($"Parsing {fileName}.pirate");
            var parseResult = _parser.StartParse(tokens, fileName);
            if (parseResult.Nodes.Count() < 1) Error("Error occurred while parsing tokens.");

            // Running Compiler
            Logger.Info($"Compiling {fileName} to {outputPath}");
            var compilationResult = _compiler.Compile(parseResult, outputPath, fileName);
            
            if (compilationResult.Success)
            {
                Console.WriteLine($"Compilation successful!");
                Console.WriteLine($"Output: {compilationResult.OutputPath}");
                
                if (compilationResult.Warnings.Any())
                {
                    Console.WriteLine("Warnings:");
                    foreach (var warning in compilationResult.Warnings)
                    {
                        Console.WriteLine($"  {warning}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Compilation failed!");
                foreach (var error in compilationResult.Errors)
                {
                    Console.WriteLine($"Error: {error}");
                }
                return false;
            }
        }
        catch (Exception ex)
        {
            Error($"Compilation failed: {ex.Message}");
        }

        return true;
    }
    
    public override void Help()
    {
        Console.WriteLine(
            new HelpOption(
                description: "pirate compile command",
                usage: "pirate compile [filename] [options]",
                options: new List<OptionDescription>()
                {
                    new OptionDescription(
                        options: new List<string>() { "-h", "--help" },
                        description: "Show command line help."
                    ),
                    new OptionDescription(
                        options: new List<string>() { "-o", "--output" },
                        description: "Specify output directory (default: ./output)"
                    )
                }
            ).ToString()
        );
    }
}