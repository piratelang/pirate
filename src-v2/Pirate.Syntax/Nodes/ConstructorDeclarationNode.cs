using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A constructor in a class file: <c>constructor(int start) { ... }</c>, or
/// one that delegates to another overload with
/// <c>constructor() : self(16) { }</c> (docs/GRAMMAR.md §4.1).
/// <see cref="DelegateArguments"/> is null when there is no <c>: self(...)</c>
/// clause, and a (possibly empty) argument list when there is.
/// </summary>
public sealed record ConstructorDeclarationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    IReadOnlyList<ParameterDefinitionNode> Parameters,
    IReadOnlyList<ExpressionNode>? DelegateArguments,
    BlockNode Body,
    bool IsPrivate)
    : TopLevelNode(StartLocation, EndLocation);
