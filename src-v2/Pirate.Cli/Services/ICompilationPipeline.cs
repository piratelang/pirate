namespace Pirate.Cli.Services;

/// <summary>
/// The CLI's compile frontend seam: runs lexer → parser → semantics over a
/// single module's source and returns the checked AST with every error the
/// stage chain collected, in stage order.
/// </summary>
public interface ICompilationPipeline
{
    FrontendResult Compile(string source);
}
