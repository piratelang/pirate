using Pirate.Syntax;

namespace Pirate.Cli.Services;

/// <summary>
/// Maps <see cref="CompilationError"/> instances to user-facing error codes.
/// The parser/lexer emit typed errors with fine-grained <c>*ErrorKind</c> enums;
/// this class is the single place where those kinds become string codes
/// (LEX-xxx, SYN-xxx, SEM-xxx) rendered to the user.
/// </summary>
public static class ErrorMapper
{
    public static string Map(CompilationError error) => error switch
    {
        LexError e     => MapLex(e.Kind),
        SyntaxError e  => MapSyntax(e.Kind),
        SemanticsError e => MapSemantics(e.Kind),
        _              => "UNK-000",
    };

    private static string MapLex(LexErrorKind kind) => kind switch
    {
        LexErrorKind.UnexpectedCharacter     => "LEX-001",
        LexErrorKind.UnterminatedStringLiteral => "LEX-002",
        LexErrorKind.UnterminatedCharLiteral  => "LEX-003",
        LexErrorKind.UnknownEscapeSequence    => "LEX-004",
        LexErrorKind.IntegerOutOfRange        => "LEX-005",
        LexErrorKind.LoneAmpersand            => "LEX-006",
        LexErrorKind.LonePipe                 => "LEX-007",
        _                                     => "LEX-999",
    };

    private static string MapSyntax(SyntaxErrorKind kind) => kind switch
    {
        // Missing tokens (SYN-001 — SYN-014)
        SyntaxErrorKind.MissingOpenBrace                    => "SYN-001",
        SyntaxErrorKind.MissingCloseBrace                   => "SYN-002",
        SyntaxErrorKind.MissingSemicolonAfterReturn         => "SYN-003",
        SyntaxErrorKind.MissingSemicolonAfterDeclaration    => "SYN-004",
        SyntaxErrorKind.MissingSemicolonAfterExpression     => "SYN-005",
        SyntaxErrorKind.MissingCloseParenAfterExpression    => "SYN-006",
        SyntaxErrorKind.MissingCloseParenAfterCall          => "SYN-007",
        SyntaxErrorKind.MissingCloseBracketAfterArrayLiteral => "SYN-008",
        SyntaxErrorKind.MissingCloseBracketAfterIndex       => "SYN-009",
        SyntaxErrorKind.MissingCloseParenInForIn            => "SYN-010",
        SyntaxErrorKind.MissingCloseBracketInType           => "SYN-011",
        SyntaxErrorKind.MissingOpenParenAfterFunctionName   => "SYN-012",
        SyntaxErrorKind.MissingCloseParenAfterParameters    => "SYN-013",
        SyntaxErrorKind.MissingColonBeforeReturnType        => "SYN-014",

        // Missing identifiers (SYN-020 — SYN-028)
        SyntaxErrorKind.MissingIdentifierAfterExtern        => "SYN-020",
        SyntaxErrorKind.MissingIdentifierAfterDot           => "SYN-021",
        SyntaxErrorKind.MissingFunctionName                 => "SYN-022",
        SyntaxErrorKind.MissingParameterName                => "SYN-023",
        SyntaxErrorKind.MissingIdentifierAfterVar           => "SYN-024",
        SyntaxErrorKind.MissingIdentifierAfterType          => "SYN-025",
        SyntaxErrorKind.MissingIdentifierInForIn            => "SYN-026",
        SyntaxErrorKind.MissingIdentifierInFor              => "SYN-027",
        SyntaxErrorKind.MissingIdentifierAfterConst         => "SYN-028",

        // Missing keywords (SYN-030 — SYN-034)
        SyntaxErrorKind.MissingVarOrParenAfterFor           => "SYN-030",
        SyntaxErrorKind.MissingInInForIn                    => "SYN-031",
        SyntaxErrorKind.MissingEqualsInDeclaration          => "SYN-032",
        SyntaxErrorKind.MissingEqualsInFor                  => "SYN-033",
        SyntaxErrorKind.MissingToInFor                      => "SYN-034",

        // Unexpected input (SYN-041 — SYN-049)
        SyntaxErrorKind.ExpectedExpression                  => "SYN-041",
        SyntaxErrorKind.UnexpectedEofInExpression           => "SYN-042",
        SyntaxErrorKind.ExpectedType                        => "SYN-043",
        SyntaxErrorKind.ExpectedImportKind                  => "SYN-044",
        SyntaxErrorKind.MissingImportName                   => "SYN-045",
        SyntaxErrorKind.MissingIdentifierAfterAs            => "SYN-046",
        SyntaxErrorKind.MissingSemicolonAfterImport         => "SYN-047",
        SyntaxErrorKind.ExpectedDeclarationAfterPrivate     => "SYN-048",
        SyntaxErrorKind.UnexpectedAliasForStandardImport    => "SYN-049",

        // Class-file shapes (SYN-050 — SYN-059)
        SyntaxErrorKind.MissingFieldOrMemberInClassFile     => "SYN-050",
        SyntaxErrorKind.MissingOpenParenAfterConstructor    => "SYN-051",
        SyntaxErrorKind.MissingCloseParenAfterConstructorParameters => "SYN-052",
        SyntaxErrorKind.MissingDelegateTargetAfterColon     => "SYN-053",
        SyntaxErrorKind.MissingOpenParenAfterDelegate       => "SYN-054",
        SyntaxErrorKind.MissingCloseParenAfterDelegateArguments => "SYN-055",
        SyntaxErrorKind.MissingIdentifierAfterNew           => "SYN-056",
        SyntaxErrorKind.MissingOpenParenAfterNew            => "SYN-057",
        SyntaxErrorKind.MissingCloseParenAfterNew           => "SYN-058",
        SyntaxErrorKind.ReservedKeywordNotSupportedYet      => "SYN-059",

        _ => "SYN-999",
    };

    private static string MapSemantics(SemanticsErrorKind kind) => kind switch
    {
        SemanticsErrorKind.UndeclaredVariable              => "SEM-001",
        SemanticsErrorKind.UndeclaredFunction              => "SEM-002",
        SemanticsErrorKind.TypeMismatch                    => "SEM-003",
        SemanticsErrorKind.AssignmentToConst               => "SEM-004",
        SemanticsErrorKind.MissingReturnInNonVoidFunction  => "SEM-005",
        SemanticsErrorKind.VoidFunctionReturnsValue        => "SEM-006",
        SemanticsErrorKind.ReturnValueRequired             => "SEM-007",
        SemanticsErrorKind.EmptyArrayRequiresElementType   => "SEM-008",
        SemanticsErrorKind.ForInIterableMustBeArray        => "SEM-009",
        SemanticsErrorKind.DuplicateDeclaration            => "SEM-010",
        SemanticsErrorKind.UnknownExtern                   => "SEM-011",
        SemanticsErrorKind.UnknownImport                   => "SEM-012",
        // SEM-013 (ModuleImportUnsupported) retired — cross-file resolution
        // exists now (docs/brainstorm/FLAT_PLAN.md Phase 3/4); the code
        // stays unused (v2 is pre-release, no dead kinds to reserve for).
        SemanticsErrorKind.ReturnAtTopLevel                => "SEM-014",
        SemanticsErrorKind.UnknownModule                   => "SEM-015",
        SemanticsErrorKind.UnknownDependency                => "SEM-016",
        SemanticsErrorKind.DependencyLocationMissing        => "SEM-017",
        SemanticsErrorKind.RemoteDependencyUnsupported      => "SEM-018",
        SemanticsErrorKind.CyclicModuleImport               => "SEM-019",
        SemanticsErrorKind.ImportedModuleHasTopLevelCode    => "SEM-020",
        SemanticsErrorKind.ImportedModuleHasErrors          => "SEM-021",
        _                                                  => "SEM-999",
    };
}
