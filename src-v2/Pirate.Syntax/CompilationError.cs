namespace Pirate.Syntax;

/// <summary>
/// Base class for all compilation errors reported by the lexer, parser, and
/// semantic analyzer. Errors are collected (not thrown) so every pipeline
/// stage can report all problems found in one pass.
/// <para>
/// The concrete subclass determines the error category (LEX, SYN, SEM) and
/// the <see cref="ErrorKind"/> enum value maps to a user-facing error code
/// in the CLI (see <c>ErrorMapper</c>).
/// </para>
/// </summary>
public abstract class CompilationError
{
    public string Message { get; }
    public SourceLocation StartLocation { get; }
    public SourceLocation? EndLocation { get; }

    protected CompilationError(string message, SourceLocation startLocation, SourceLocation? endLocation = null)
    {
        Message = message;
        StartLocation = startLocation;
        EndLocation = endLocation;
    }

    public override string ToString() => Message;
}
