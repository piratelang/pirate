using System.Text.Json.Serialization;

namespace Pirate.Fleet;

/// <summary>
/// One entry of the ".fleet" manifest's "dependencies" map
/// (docs/FLEET.md) — where <c>import external &lt;dep&gt;.&lt;path&gt;;</c>
/// resolves its root module from (docs/GRAMMAR.md §4.4).
/// <see cref="Location"/> is either a path relative to the project root
/// (resolved by the module linker) or an <c>http(s)</c> URL (declared but
/// not fetched yet — importing one reports a diagnostic).
/// </summary>
public sealed class FleetDependency
{
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;
}
