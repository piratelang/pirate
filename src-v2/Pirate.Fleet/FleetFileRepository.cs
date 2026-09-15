using System.Text.Json;
using Pirate.Shared.File;

namespace Pirate.Fleet;

/// <summary>Reads and writes a project's ".fleet" manifest.</summary>
public static class FleetFileRepository
{
    private static readonly JsonSerializerOptions WriteOptions = new() { WriteIndented = true };

    public static bool Exists(string directory) =>
        FleetFileLocator.DiscoverFleetFiles(directory).Count > 0;

    /// <summary>
    /// Returns null if no "*.fleet" file exists in <paramref name="directory"/>.
    /// If more than one exists, reads the first alphabetically — a directory
    /// is only expected to have one. Throws <see cref="JsonException"/> if
    /// the file that's found isn't valid JSON — that's an actionable user
    /// error the caller should report clearly, not a case to silently
    /// swallow.
    /// </summary>
    public static FleetFile? TryRead(string directory)
    {
        var path = FleetFileLocator.DiscoverFleetFiles(directory).FirstOrDefault();
        if (path is null)
        {
            return null;
        }

        var json = File.ReadAllText(path);
        try
        {
            return JsonSerializer.Deserialize<FleetFile>(json)
                ?? throw new JsonException("deserialized to null");
        }
        catch (JsonException ex)
        {
            throw new JsonException($"\"{Path.GetFileName(path)}\": {ex.Message}", ex);
        }
    }

    /// <summary>Writes "{name}.fleet" (name without extension) in <paramref name="directory"/>.</summary>
    public static void Write(string directory, string name, FleetFile fleet)
    {
        var path = Path.Combine(directory, $"{name}.fleet");
        File.WriteAllText(path, JsonSerializer.Serialize(fleet, WriteOptions));
    }
}
