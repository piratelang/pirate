using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Pirate.Fleet;

/// <summary>
/// The ".fleet" project manifest — a package.json-style file identifying a
/// directory as a Pirate project. "Build" is a reserved, empty-shaped
/// placeholder: it has no consumer yet (there's no compiler to configure),
/// so it stays an untyped JSON object that round-trips whatever is there
/// rather than committing to a schema with no basis yet. "Dependencies"
/// (docs/FLEET.md) is typed: the module linker (docs/brainstorm/FLAT_PLAN.md
/// Phase 3) resolves <c>import external</c> against it.
/// </summary>
public sealed class FleetFile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = "0.1.0";

    [JsonPropertyName("entryPoint")]
    public string EntryPoint { get; set; } = "main";

    [JsonPropertyName("build")]
    public JsonObject Build { get; set; } = new();

    [JsonPropertyName("dependencies")]
    public Dictionary<string, FleetDependency> Dependencies { get; set; } = new();
}
