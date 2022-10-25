using Common.Errors;
using Microsoft.Extensions.Configuration;

namespace Common
{
    public class EnvironmentVariables
    {
        public static string GetVariable(string variablename)
        {
            try
            {
                var directory = Directory.GetCurrentDirectory();
                var secretConfig = new ConfigurationBuilder()
                    .AddJsonFile($"{directory}/../Shell/variables.json", false, true)
                    .Build();
                return secretConfig[variablename];
            }
            catch (System.Exception)
            {
                throw new FileException($"{variablename} was not found in variables.json");
            }
        }
    }
}