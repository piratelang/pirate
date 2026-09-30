namespace Pirate.Syntax.Nodes;

/// <summary>
/// Base class for all expression nodes. Expressions produce a value — unlike
/// statements, which perform an action.
/// </summary>
public abstract record ExpressionNode(SourceLocation StartLocation, SourceLocation EndLocation)
{
    /// <summary>
    /// Set by the semantics pass after a successful check; null before it
    /// runs and for expressions that failed to type-check. The compiler
    /// consumes this instead of re-deriving types.
    /// </summary>
    public PirateType? InferredType { get; set; }
}

/// <summary>
/// Base class for all statement nodes. Statements perform actions and do not
/// produce a value — unlike expressions.
/// </summary>
public abstract record StatementNode(SourceLocation StartLocation, SourceLocation EndLocation);

/// <summary>
/// Base class for top-level nodes (program, function declarations, extern
/// declarations). These appear outside of any function body.
/// </summary>
public abstract record TopLevelNode(SourceLocation StartLocation, SourceLocation EndLocation);
