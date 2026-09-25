namespace Pirate.Syntax;

/// <summary>
/// Fine-grained syntax error kinds. Each value maps to exactly one
/// <c>SYN-xxx</c> error code in the CLI's <c>ErrorMapper</c>.
/// <para>
/// Numbering convention (gaps allow inserting new errors without renumbering):
/// 001-014: Missing closing/separator tokens
/// 020-027: Missing identifiers
/// 030-034: Missing keywords
/// 040-043: Unexpected input
/// </para>
/// </summary>
public enum SyntaxErrorKind
{
    // Missing tokens (SYN-001 — SYN-014)
    MissingOpenBrace,
    MissingCloseBrace,
    MissingSemicolonAfterReturn,
    MissingSemicolonAfterDeclaration,
    MissingSemicolonAfterExpression,
    MissingCloseParenAfterExpression,
    MissingCloseParenAfterCall,
    MissingCloseBracketAfterArrayLiteral,
    MissingCloseBracketAfterIndex,
    MissingCloseParenInForIn,
    MissingCloseBracketInType,
    MissingOpenParenAfterFunctionName,
    MissingCloseParenAfterParameters,
    MissingColonBeforeReturnType,

    // Missing identifiers (SYN-020 — SYN-027)
    MissingIdentifierAfterExtern,
    MissingIdentifierAfterDot,
    MissingFunctionName,
    MissingParameterName,
    MissingIdentifierAfterVar,
    MissingIdentifierAfterType,
    MissingIdentifierInForIn,
    MissingIdentifierInFor,

    // Missing keywords (SYN-030 — SYN-034)
    MissingVarOrParenAfterFor,
    MissingInInForIn,
    MissingEqualsInDeclaration,
    MissingEqualsInFor,
    MissingToInFor,

    // Unexpected input (SYN-040 — SYN-043)
    ExpectedTopLevelDeclaration,
    ExpectedExpression,
    UnexpectedEofInExpression,
    ExpectedType,
}

/// <summary>
/// A syntax error reported by <c>Pirate.Parser</c>.
/// </summary>
public sealed class SyntaxError : CompilationError
{
    public SyntaxErrorKind Kind { get; }

    public SyntaxError(SyntaxErrorKind kind, string message, SourceLocation startLocation, SourceLocation? endLocation = null)
        : base(message, startLocation, endLocation)
    {
        Kind = kind;
    }
}
