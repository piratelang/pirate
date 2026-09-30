namespace Pirate.Shared.File;

/// <summary>
/// One discovered Pirate source file: its path, which kind its extension
/// names, and the type name the filename spells (the filename without its
/// final extension — docs/GRAMMAR.md §4). <see cref="TypeName"/> is
/// whatever text preceded the extension, even when it isn't a single
/// identifier (e.g. <c>foo.bar</c> from <c>foo.bar.cpirate</c>) — callers
/// that need to reject that shape check it themselves; this type only
/// reports what's on disk.
/// </summary>
public sealed record PirateSourceFile(string Path, PirateFileExtensionKind Kind, string TypeName);
