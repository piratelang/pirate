using Pirate.Common.Errors;
using Microsoft.Extensions.Configuration;
using Pirate.Common.Interfaces;
using Pirate.Common.FileHandler.Model;
using Pirate.Common.FileHandler.Enum;
using Pirate.Common.FileHandler.Interfaces;
using Pirate.Common.Exception;

namespace Pirate.Common;

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
        var configDirectory = GetConfigDirectory();
        CreateTemplateVariablesFile(configDirectory);

        Configuration = new ConfigurationBuilder()
            .AddJsonFile(Path.Combine(configDirectory, "variables.json"), false, true)
            .Build();
    }

    public string GetVariable(string variablename)
    {
        if (variablename is null) throw new ArgumentNullException(nameof(variablename));
        var variable = Configuration[variablename];
        if (variable is null)
            throw new FileException(new ExceptionCode("COMMON", "001"), new List<string> { variablename });

        return variable;
    }

    private string GetConfigDirectory()
    {
        var xdgConfigHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        var configDirectory = string.IsNullOrWhiteSpace(xdgConfigHome)
            ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            : xdgConfigHome;

        return Path.Combine(configDirectory, "pirate");
    }

    private void CreateTemplateVariablesFile(string configDirectory)
    {
        FileWriteHandler.WriteToFile(
            new FileWriteModel(
                "variables",
                FileExtension.JSON,
                configDirectory,
                string.Join(
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