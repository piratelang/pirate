namespace Pirate.Cli.Services;

/// <summary>
/// A module graph node: its origin (project file vs. manifest dependency)
/// plus its dotted name. Project and external modules live in separate name
/// spaces, so the pair — not the name alone — identifies a module.
/// </summary>
public readonly record struct ModuleId(ModuleOrigin Origin, string Name)
{
    public override string ToString() => Origin == ModuleOrigin.External ? $"external '{Name}'" : $"'{Name}'";
}
