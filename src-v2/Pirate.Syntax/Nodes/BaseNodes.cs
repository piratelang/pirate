namespace Pirate.Syntax.Nodes;

/// <summary>
/// Base class for all expression nodes. Expressions produce a value — unlike
/// statements, which perform an action.
/// </summary>
public abstract record ExpressionNode(SourceLocation StartLocation, SourceLocation EndLocation);

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
