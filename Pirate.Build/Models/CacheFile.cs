using Pirate.Parser;

namespace Pirate.Build.Models;

public class CacheFile
{
    public Dictionary<string, Scope> Cache { get; set; } = new();
}
