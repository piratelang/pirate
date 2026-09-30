namespace Pirate.Shared.File;

/// <summary>
/// Resolves a user-supplied filename argument to the module name used on
/// disk, matching v1's behavior: default to "main" when omitted, and accept
/// the name with or without the ".pirate" extension.
/// </summary>
public static class PirateFileName
{
    private const string DefaultName = "main";
    private const string Extension = ".pirate";

    public static string Resolve(string? argument) =>
        FileNameResolver.Resolve(argument, DefaultName, Extension);
}
