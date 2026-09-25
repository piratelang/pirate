using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pirate.Cli.Services;

/// <summary>
/// Content-hash-based build cache stored in <c>.pirate/cache.json</c>.
/// Only modules whose file contents have changed (SHA256 mismatch) are
/// rebuilt, catching even manual reverts that would fool LastWriteTime.
/// </summary>
public class BuildCache
{
    private readonly string _rootDirectory;
    private readonly string _cachePath;
    private CacheData _data;

    private BuildCache(string rootDirectory, string cachePath, CacheData data)
    {
        _rootDirectory = rootDirectory;
        _cachePath = cachePath;
        _data = data;
    }

    private static CacheData Empty() =>
        new() { Modules = new Dictionary<string, ModuleCache>(StringComparer.Ordinal) };

    /// <summary>
    /// Loads the cache from disk, or creates a new empty one if none exists.
    /// A corrupt or unreadable cache file is treated as empty — the cost of
    /// a full rebuild is preferable to crashing the CLI.
    /// </summary>
    public static BuildCache Load(string projectDirectory)
    {
        var cachePath = Path.Combine(projectDirectory, ".pirate", "cache.json");

        if (!File.Exists(cachePath))
        {
            return new BuildCache(projectDirectory, cachePath, Empty());
        }

        try
        {
            var json = File.ReadAllText(cachePath);
            var data = JsonSerializer.Deserialize<CacheData>(json);
            return new BuildCache(projectDirectory, cachePath, data?.Modules is null ? Empty() : data);
        }
        catch (Exception ex) when (ex is JsonException or IOException)
        {
            return new BuildCache(projectDirectory, cachePath, Empty());
        }
    }

    /// <summary>
    /// Computes the SHA256 hash of a file's contents.
    /// </summary>
    public static string HashFile(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha.ComputeHash(stream);
        return $"sha256:{Convert.ToHexString(hash).ToLowerInvariant()}";
    }

    /// <summary>
    /// Returns true if the file's current hash matches the cached hash.
    /// </summary>
    public bool IsUpToDate(string filePath)
    {
        var relative = Path.GetRelativePath(_rootDirectory, filePath);
        if (!_data.Modules.TryGetValue(relative, out var entry)) return false;
        var currentHash = HashFile(filePath);
        return entry.Hash == currentHash;
    }

    /// <summary>
    /// Records the file's current hash and timestamp in the cache.
    /// </summary>
    public void MarkBuilt(string filePath)
    {
        var relative = Path.GetRelativePath(_rootDirectory, filePath);
        _data.Modules[relative] = new ModuleCache
        {
            Hash = HashFile(filePath),
            LastBuilt = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Writes the cache to disk atomically (temp file + rename), creating
    /// the <c>.pirate</c> directory if needed. A crash mid-write then leaves
    /// the previous valid cache intact.
    /// </summary>
    public void Save()
    {
        var dir = Path.GetDirectoryName(_cachePath)!;
        Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
        var tempPath = _cachePath + ".tmp";
        File.WriteAllText(tempPath, json);
        File.Move(tempPath, _cachePath, overwrite: true);
    }

    private class CacheData
    {
        [JsonPropertyName("modules")]
        public Dictionary<string, ModuleCache> Modules { get; init; } = new(StringComparer.Ordinal);
    }

    private class ModuleCache
    {
        [JsonPropertyName("hash")]
        public string Hash { get; set; } = string.Empty;

        [JsonPropertyName("lastBuilt")]
        public DateTime LastBuilt { get; set; }
    }
}
