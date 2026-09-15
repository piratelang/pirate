namespace Pirate.Shared.File;

/// <summary>
/// Resolves a user-supplied name argument to a base file name: default to
/// <paramref name="defaultName"/> when omitted, and accept the name with or
/// without <paramref name="extension"/>. Shared by
/// <see cref="PirateFileName"/> (".pirate"/"main") and Pirate.Fleet's
/// FleetFileName (".fleet"/"module").
/// </summary>
public static class FileNameResolver
{
    public static string Resolve(string? argument, string defaultName, string extension)
    {
        var name = string.IsNullOrWhiteSpace(argument) ? defaultName : argument;
        return name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)
            ? name[..^extension.Length]
            : name;
    }
}
