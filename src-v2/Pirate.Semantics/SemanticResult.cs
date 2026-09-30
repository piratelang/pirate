using System.Collections.Generic;
using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics;

/// <summary>
/// The semantics pass's output: the annotated (checked) program plus any
/// <see cref="SemanticsError"/>s found. Matches the <c>LexResult</c> /
/// <c>ParseResult</c> shape — collect-don't-throw, and the program node is
/// always returned so downstream tooling can inspect a partial model.
/// Errors from earlier stages are not merged in here; the CLI concatenates
/// them (v2-017).
/// </summary>
public sealed class SemanticResult
{
    public ProgramNode Program { get; }

    public IReadOnlyList<SemanticsError> Errors { get; }

    public SemanticResult(ProgramNode program, IReadOnlyList<SemanticsError> errors)
    {
        Program = program;
        Errors = errors;
    }
}
