using Pirate.Semantics;
using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Cli.Services;

/// <summary>
/// Output of the compile frontend for a single module: the checked AST (null
/// when the pipeline failed too early to produce one) and every error the
/// stages found, in stage order.
/// </summary>
public sealed record FrontendResult(ProgramNode? Program, IReadOnlyList<CompilationError> Errors)
{
    public bool Success => Errors.Count == 0;
}

/// <summary>
/// The compile frontend: lexer → parser → semantics, concatenating each
/// stage's errors into one list (each stage reports only its own errors —
/// the CLI concatenates, per v2-017) so everything a module got wrong is
/// shown in a single pass. Semantics runs only on a clean parse: the
/// analyzer assumes a well-formed AST, and cascading over partial parses
/// would double-report.
///
/// Lexer/Parser are fully qualified because inside Pirate.* namespaces the
/// simple names bind to the namespaces, not the classes; their conversion to
/// injected services is tracked as STYLE.md migration item (b).
/// </summary>
internal sealed class CompilationPipeline(ISemanticAnalyzer semanticAnalyzer) : ICompilationPipeline
{
    public FrontendResult Compile(string source)
    {
        var lex = Pirate.Lexer.Lexer.Tokenize(source);
        var parse = Pirate.Parser.Parser.Parse(lex);

        List<CompilationError> errors = [.. lex.Errors, .. parse.Errors];

        ProgramNode? program = null;
        // F# option: IsSome is not a C#-friendly property, use the accessor.
        if (errors.Count == 0 && parse.Program is { } parsed && Microsoft.FSharp.Core.FSharpOption<ProgramNode>.get_IsSome(parsed))
        {
            program = parsed.Value;
            var semantics = semanticAnalyzer.Analyze(program);
            errors.AddRange(semantics.Errors);
        }

        // Concatenated per stage, then rendered in source order (stable:
        // same-position errors keep stage order) — a compiler reads top
        // down, like the diagnostics of the tools it replaces.
        IReadOnlyList<CompilationError> ordered =
            [.. errors.OrderBy(error => error.StartLocation.Line).ThenBy(error => error.StartLocation.Column)];
        return new FrontendResult(program, ordered);
    }
}
