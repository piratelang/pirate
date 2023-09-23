using Microsoft.Extensions.DependencyInjection;
using Pirate.Build.Project.Interfaces;

namespace Pirate.Build.Project;

public class Register
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddTransient<IProjectFileHandler, ProjectFileHandler>();
    }
}
