using Pirate.Syntax.Nodes;

namespace Pirate.Cli.Services;

/// <summary>
/// One <c>import module</c>/<c>import external</c> edge of the module
/// graph: the importing module, the module it imports, and the import
/// statement the edge was built from (the site any link diagnostic is
/// reported at, and the node the linker annotates with its resolution).
/// <see cref="Site"/> is null for an implicit same-folder edge
/// (docs/GRAMMAR.md §4.4) — there is no import statement to annotate.
/// </summary>
internal sealed record ImportEdge(ModuleId Importer, ModuleId Target, ImportStatementNode? Site);
