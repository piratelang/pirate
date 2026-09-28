using System.Security.Cryptography;
using System.Text.Json;
using Pirate.Cli.Services;

namespace Pirate.Cli.Test.Services;

public class BuildCacheTests : IDisposable
{
    private readonly string _root;

    public BuildCacheTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "pirate-cache-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);

    private string WriteModule(string name, string content)
    {
        var path = Path.Combine(_root, name);
        File.WriteAllText(path, content);
        return path;
    }

    private string CachePath => Path.Combine(_root, ".pirate", "cache.json");

    [Fact]
    public void MarkBuilt_SaveReload_FileIsUpToDate()
    {
        var module = WriteModule("main.pirate", "var x = 5;");

        var cache = BuildCache.Load(_root);
        cache.MarkBuilt(module);
        cache.Save();

        Assert.True(BuildCache.Load(_root).IsUpToDate(module));
    }

    [Fact]
    public void ContentChanged_FileIsNotUpToDate()
    {
        var module = WriteModule("main.pirate", "var x = 5;");
        var cache = BuildCache.Load(_root);
        cache.MarkBuilt(module);
        cache.Save();

        File.WriteAllText(module, "var x = 6;");

        Assert.False(BuildCache.Load(_root).IsUpToDate(module));
    }

    [Fact]
    public void StalePipelineVersion_CacheIsDiscarded()
    {
        var module = WriteModule("main.pirate", "var x = 5;");

        // Hand-write a cache claiming an older pipeline version, with a hash
        // that matches the file's current content — a version-blind cache
        // would call this up to date.
        var hash = ComputeSha256(module);
        Directory.CreateDirectory(Path.GetDirectoryName(CachePath)!);
        File.WriteAllText(CachePath, JsonSerializer.Serialize(new
        {
            pipelineVersion = BuildCache.PipelineVersion - 1,
            modules = new Dictionary<string, object>
            {
                ["main.pirate"] = new { hash, lastBuilt = DateTime.UtcNow },
            },
        }));

        Assert.False(BuildCache.Load(_root).IsUpToDate(module));
    }

    [Fact]
    public void MissingPipelineVersion_CacheIsDiscarded()
    {
        var module = WriteModule("main.pirate", "var x = 5;");
        var hash = ComputeSha256(module);

        Directory.CreateDirectory(Path.GetDirectoryName(CachePath)!);
        File.WriteAllText(CachePath, JsonSerializer.Serialize(new
        {
            modules = new Dictionary<string, object>
            {
                ["main.pirate"] = new { hash, lastBuilt = DateTime.UtcNow },
            },
        }));

        Assert.False(BuildCache.Load(_root).IsUpToDate(module));
    }

    [Fact]
    public void SavedCache_CarriesCurrentPipelineVersion()
    {
        var module = WriteModule("main.pirate", "var x = 5;");
        var cache = BuildCache.Load(_root);
        cache.MarkBuilt(module);
        cache.Save();

        using var document = JsonDocument.Parse(File.ReadAllText(CachePath));
        Assert.Equal(BuildCache.PipelineVersion, document.RootElement.GetProperty("pipelineVersion").GetInt32());
    }

    [Fact]
    public void CorruptCacheFile_LoadTreatsItAsEmpty()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(CachePath)!);
        File.WriteAllText(CachePath, "{ not json ]");

        var cache = BuildCache.Load(_root);

        Assert.False(cache.IsUpToDate(WriteModule("main.pirate", "var x = 5;")));
    }

    private static string ComputeSha256(string filePath)
    {
        using var sha = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        return $"sha256:{Convert.ToHexString(sha.ComputeHash(stream)).ToLowerInvariant()}";
    }
}
