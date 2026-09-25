using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// An extern declaration: <c>extern Standard.Terminal.Print;</c>. Makes a
/// standard-library function available by its dotted path. The parser stores
/// the qualified name as-is; name resolution happens in the semantics pass.
/// </summary>
public sealed record ExternNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<string> QualifiedName)
    : TopLevelNode(StartLocation, EndLocation);
