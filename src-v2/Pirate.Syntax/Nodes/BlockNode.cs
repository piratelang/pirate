using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A block of statements enclosed in braces: <c>{ stmt1; stmt2; return x; }</c>.
/// Appears as function bodies, if/else branches, and loop bodies. Per the
/// grammar (§3.3), a block contains zero or more statements followed by an
/// optional return statement.
/// </summary>
public sealed record BlockNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<StatementNode> Statements,
    ReturnStatementNode? ReturnStatement)
    : StatementNode(StartLocation, EndLocation);
