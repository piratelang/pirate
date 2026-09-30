namespace Pirate.Syntax.Nodes;

/// <summary>
/// A field or class constant in a class file: <c>field int count = 0;</c>,
/// <c>readonly field int count = 0;</c>, <c>private field int calls = 0;</c>,
/// <c>const int Limit = 1;</c> (docs/GRAMMAR.md §4.1). <see cref="Type"/> is
/// null only for <c>field var label = ...</c> (inferred, same rule as a
/// local <c>var</c>) and for a type-less <c>const</c>.
/// <see cref="Initializer"/> is null when the field has none — legal only
/// when every constructor assigns it (definite assignment, checked by
/// semantics, not the parser). <see cref="IsReadonly"/> is rejected by
/// semantics on <see cref="IsConst"/> — a constant can't be assigned at all.
/// </summary>
public sealed record FieldDeclarationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    TypeNode? Type,
    bool IsConst,
    bool IsPrivate,
    bool IsReadonly,
    string Name,
    ExpressionNode? Initializer)
    : TopLevelNode(StartLocation, EndLocation);
