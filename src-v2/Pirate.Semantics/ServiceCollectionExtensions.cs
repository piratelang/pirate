using Microsoft.Extensions.DependencyInjection;

namespace Pirate.Semantics;

/// <summary>
/// Per-project service registration for the semantics stage. The analyzer
/// resets all mutable state at the start of each <c>Analyze</c> call, so a
/// single instance is safe as the app-wide singleton.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPirateSemantics(this IServiceCollection services)
    {
        services.AddSingleton<ISemanticAnalyzer, SemanticAnalyzer>();
        return services;
    }
}
