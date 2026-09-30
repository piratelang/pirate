using Microsoft.Extensions.DependencyInjection;

namespace Pirate.Lexer;

/// <summary>
/// Per-project service registration for the lexer stage. The scanner is
/// stateless between calls (all state lives in the per-call <c>Scanner</c>),
/// so a single instance is safe as the app-wide singleton.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPirateLexer(this IServiceCollection services)
    {
        services.AddSingleton<ILexer, Lexer>();
        return services;
    }
}
