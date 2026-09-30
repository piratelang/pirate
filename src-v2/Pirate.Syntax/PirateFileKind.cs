namespace Pirate.Syntax;

/// <summary>
/// Which of Pirate's file kinds (docs/GRAMMAR.md §4) is being parsed. Only
/// <see cref="Module"/> has a grammar today — <see cref="Class"/> and
/// <see cref="Interface"/> are threaded through ahead of Phase 2's
/// class-file grammar (docs/brainstorm/FLAT_PLAN.md) so the parser's
/// interface already has the seam the later grammar needs, rather than
/// changing the signature again once it lands.
/// </summary>
public enum PirateFileKind
{
    /// <summary>
    /// A <c>.pirate</c>/<c>.pir</c> file: functions and constants, plus
    /// top-level statements when it is the project's entry module.
    /// </summary>
    Module,

    /// <summary>
    /// A <c>.cpirate</c>/<c>.cpir</c> file: fields, constructors, and
    /// methods, no loose statements. Not yet implemented.
    /// </summary>
    Class,

    /// <summary>
    /// A <c>.ipirate</c>/<c>.ipir</c> file: bodiless method signatures.
    /// Reserved, not yet implemented.
    /// </summary>
    Interface,
}
