using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Pirate.Cli.Services;

/// <summary>
/// Bridges Spectre.Console.Cli's command activation onto
/// Microsoft.Extensions.DependencyInjection, so commands receive their
/// dependencies via constructor injection instead of news up pipeline
/// stages themselves.
/// </summary>
internal sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
{
    public ITypeResolver Build() => new TypeAdapter(services.BuildServiceProvider());

    public void Register(Type service, Type implementation) => services.AddSingleton(service, implementation);

    public void RegisterInstance(Type service, object implementation) => services.AddSingleton(service, implementation);

    public void RegisterLazy(Type service, Func<object> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        services.AddSingleton(service, _ => factory());
    }
}
