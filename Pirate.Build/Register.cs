using Microsoft.Extensions.DependencyInjection;

namespace Pirate.Build;

public class Register
{
    public static void RegisterDependencies(IServiceCollection builder)
    {
        builder.AddTransient<IRunAction, RunAction>();
        builder.AddTransient<IBuildAction, BuildAction>();
    }
}