using Pirate.Shared.File;

namespace Pirate.Fleet;

/// <summary>
/// Resolves the module "pirate run"/"pirate init" should treat as the entry
/// point: an explicit argument always wins; otherwise a ".fleet" in
/// <paramref name="directory"/> supplies it; otherwise "main"
/// (<see cref="PirateFileName"/>'s own default).
/// </summary>
public static class FleetEntryPoint
{
    public static string Resolve(string? argument, string directory)
    {
        if (!string.IsNullOrWhiteSpace(argument))
        {
            return PirateFileName.Resolve(argument);
        }

        var fleet = FleetFileRepository.TryRead(directory);
        return PirateFileName.Resolve(fleet?.EntryPoint);
    }
}
