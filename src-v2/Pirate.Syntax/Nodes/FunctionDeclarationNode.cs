using System.Collections.Generic;

namespace Pirate.Syntax.Nodes;

/// <summary>
/// A function declaration: <c>func main() : void { ... }</c> or
/// <c>func add(int a, int b) : int { return a + b; }</c>.
/// <see cref="ReturnType"/> is never null — void-returning functions use
/// <see cref="ScalarType.Void"/>. <see cref="IsExported"/> marks a function
/// declared with <c>export func</c>; its visibility is consumed by the
/// module linker once that lands.
/// </summary>
public sealed record FunctionDeclarationNode(
    SourceLocation StartLocation,
    SourceLocation EndLocation,
    string Name,
    IReadOnlyList<ParameterDefinitionNode> Parameters,
    TypeNode ReturnType,
    BlockNode Body,
    bool IsExported)
    : TopLevelNode(StartLocation, EndLocation);
