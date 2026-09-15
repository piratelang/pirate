namespace Pirate.Shared.File;

/// <summary>
/// Resolves a user-supplied name argument to the ".fleet" manifest's base
/// file name: default to "module" when omitted, and accept the name with
/// or without the ".fleet" extension — same shape as
/// <see cref="PirateFileName"/>, for the manifest instead of a module.
/// </summary>
public static class FleetFileName
{
    private const string DefaultName = "module";
    private const string Extension = ".fleet";

    public static string Resolve(string? argument) =>
        FileNameResolver.Resolve(argument, DefaultName, Extension);
}
