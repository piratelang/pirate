using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Pirate.Fleet;

/// <summary>
/// The ".fleet" project manifest — a package.json-style file identifying a
/// directory as a Pirate project. "Build"/"Dependencies" are reserved,
/// empty-shaped placeholders: neither has a real consumer yet (there's no
/// compiler and no package ecosystem), so they stay untyped JSON objects
/// that round-trip whatever is there rather than committing to a schema
/// with no basis yet.
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
    public JsonObject Dependencies { get; set; } = new();
}
