namespace Pirate.Shared.File;

/// <summary>
/// Which of Pirate's file kinds a discovered file's extension names
/// (docs/GRAMMAR.md §4): a module (<c>.pirate</c>/<c>.pir</c>), a class
/// (<c>.cpirate</c>/<c>.cpir</c>), or an interface
/// (<c>.ipirate</c>/<c>.ipir</c>, reserved — no grammar yet).
/// </summary>
public enum PirateFileExtensionKind
{
    Module,
    Class,
    Interface,
}
