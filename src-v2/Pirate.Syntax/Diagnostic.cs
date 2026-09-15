namespace Pirate.Syntax;

/// <summary>
/// A single error or warning collected during lexing, parsing, or semantic
/// analysis. Every pipeline stage reports problems this way instead of
/// throwing, so a whole file's worth of diagnostics can be printed in one
/// pass.
/// </summary>
public readonly record struct Diagnostic(string Message, SourceLocation Location);
