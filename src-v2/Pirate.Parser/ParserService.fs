namespace Pirate.Parser

open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection

/// <summary>
/// DI-friendly wrapper around the <see cref="Parser"/> module's
/// <c>Parse</c> function (docs/STYLE.md migration item (b)). The module
/// function stays the actual parser, matching F#'s v2-native
/// module-of-functions shape (docs/STYLE.md); this class only adapts it to
/// <see cref="IParser"/> so <c>Pirate.Cli</c> can inject it instead of
/// calling the static entry point directly.
/// </summary>
type ParserService() =
    interface IParser with
        member _.Parse(lexResult, fileKind) = Parser.Parse lexResult fileKind

/// <summary>Per-project service registration for the parser stage.</summary>
[<Extension>]
type ServiceCollectionExtensions =
    [<Extension>]
    static member AddPirateParser(services: IServiceCollection) : IServiceCollection =
        services.AddSingleton<IParser, ParserService>() |> ignore
        services
