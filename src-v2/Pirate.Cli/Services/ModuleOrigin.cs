namespace Pirate.Cli.Services;

/// <summary>
/// Where a module in the linker's graph comes from: <see cref="Project"/>
/// for a project source file (reached by <c>import module</c>),
/// <see cref="External"/> for a dependency declared in the <c>.fleet</c>
/// manifest (reached by <c>import external</c>). The two origins are
/// separate name spaces — a project module and an external dependency may
/// share a name without colliding.
/// </summary>
public enum ModuleOrigin
{
    Project,
    External,
}
