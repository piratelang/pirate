namespace Pirate.Syntax;

/// <summary>
/// A 1-based line/column position in a source file. Shared by every pipeline
/// stage (lexer, parser, semantics) that needs to point a diagnostic at real
/// source text.
/// </summary>
public readonly record struct SourceLocation(int Line, int Column);
