using Spectre.Console.Cli;

namespace Pirate.Cli.Services;

/// <summary>
/// Resolves Spectre-created command dependencies from the built
/// <see cref="System.IServiceProvider"/>; see <see cref="TypeRegistrar"/>.
/// </summary>
internal sealed class TypeAdapter(System.IServiceProvider provider) : ITypeResolver, IDisposable
{
    public object? Resolve(Type? type) => type is null ? null : provider.GetService(type);

    public void Dispose()
    {
        if (provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
