using Common.Enum;
using Common.Errors;
using Common.FileHandlers;
using Common.FileHandlers.Interfaces;
using Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Common
{

    public class EnvironmentVariables : IEnvironmentVariables
    {
        public IFileReadHandler FileReadHandler { get; set; }
        public IFileWriteHandler FileWriteHandler { get; set; }

        public EnvironmentVariables(IFileReadHandler fileReadHandler, IFileWriteHandler fileWriteHandler)
        {
            FileReadHandler = fileReadHandler;
            FileWriteHandler = fileWriteHandler;
        }

        public string GetVariable(string variablename)
        {

            var directory = Directory.GetCurrentDirectory();
            if (variablename is null)
            {
                throw new ArgumentNullException(nameof(variablename));
            }
            if (!FileReadHandler.FileExists("variables", FileExtension.JSON, $"{directory}/bin"))
            {
                CreateTemplateVariablesFile(directory);
            }

            try
            {
                var secretConfig = new ConfigurationBuilder()
                    .AddJsonFile($"{directory}/bin/variables.json", false, true)
                    .Build();
                return secretConfig[variablename];
            }
            catch (System.Exception)
            {
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
}