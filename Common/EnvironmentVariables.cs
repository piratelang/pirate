using Common.Enum;
using Common.Errors;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Common;

/// <summary>
/// This class is used to get the environment variables from the variables.json file.
/// </summary>
public class EnvironmentVariables : IEnvironmentVariables
{
    public IFileReadHandler FileReadHandler { get; set; }
    public IFileWriteHandler FileWriteHandler { get; set; }
    public IConfiguration Configuration { get; set; }

    public EnvironmentVariables(IFileReadHandler fileReadHandler, IFileWriteHandler fileWriteHandler)
    {
        FileReadHandler = fileReadHandler;
        FileWriteHandler = fileWriteHandler;
        var directory = Directory.GetCurrentDirectory();
        if (!FileReadHandler.FileExists("variables", FileExtension.JSON, $"{directory}/bin"))
        {
            CreateTemplateVariablesFile(directory);
        }

        Configuration = new ConfigurationBuilder()
            .AddJsonFile($"{directory}/bin/variables.json", false, true)
            .Build();
    }

    public string GetVariable(string variablename)
    {
        if (variablename is null) throw new ArgumentNullException(nameof(variablename));
        try
        {
            return Configuration[variablename];
        }
        catch (System.Exception)
        {
            Console.WriteLine($"Failed to get variable \"{variablename}\" from variables.json");
            throw new FileException($"{variablename} was not found in variables.json");
        }
    }

    private void CreateTemplateVariablesFile(string directory)
    {
        FileWriteHandler.WriteToFile(
            new FileWriteModel(
                "variables",
                FileExtension.JSON,
                $"{directory}/bin/",
                String.Join(
                    Environment.NewLine,
                    "{",
                    "     \"version\": \"1.1.0\",",
                    "     \"location\" : \"bin/pirate1.1.0\"",
                    "}"
                )
            )
        );
    }
}