namespace Pirate.Syntax;

/// <summary>
/// Fine-grained syntax error kinds. Each value maps to exactly one
/// <c>SYN-xxx</c> error code in the CLI's <c>ErrorMapper</c>.
/// <para>
/// Numbering convention (gaps allow inserting new errors without renumbering):
/// 001-014: Missing closing/separator tokens
/// 020-028: Missing identifiers
/// 030-034: Missing keywords
/// 041-048: Unexpected input, including import/export shapes
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

    // Missing identifiers (SYN-020 — SYN-028)
    MissingIdentifierAfterExtern,
    MissingIdentifierAfterDot,
    MissingFunctionName,
    MissingParameterName,
    MissingIdentifierAfterVar,
    MissingIdentifierAfterType,
    MissingIdentifierInForIn,
    MissingIdentifierInFor,
    MissingIdentifierAfterConst,

    // Missing keywords (SYN-030 — SYN-034)
    MissingVarOrParenAfterFor,
    MissingInInForIn,
    MissingEqualsInDeclaration,
    MissingEqualsInFor,
    MissingToInFor,

    // Unexpected input (SYN-041 — SYN-049)
    ExpectedExpression,
    UnexpectedEofInExpression,
    ExpectedType,
    ExpectedImportKind,
    MissingImportName,
    MissingIdentifierAfterAs,
    MissingSemicolonAfterImport,
    ExpectedDeclarationAfterExport,
    UnexpectedAliasForStandardImport,
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
