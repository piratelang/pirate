using Pirate.Syntax.Nodes;

namespace Pirate.Cli.Services;

/// <summary>
/// The module import graph: nodes are parsed modules/class files (project
/// files and resolved external dependencies), edges are their
/// <c>import module</c>/<c>import external</c> statements plus implicit
/// same-folder edges (docs/GRAMMAR.md §4.4). Pure data and traversal — no
/// I/O, no diagnostics. The linker asks it which import edges participate
/// in cycles (those can never be handed an interface), for a deterministic
/// dependencies-first analysis order over the remaining edges, and for a
/// module's import closure.
/// </summary>
internal sealed class ModuleGraph
{
    /// <summary>
    /// Deterministic node order: project modules before externals, names
    /// ordinal — so analysis order, cycle paths, and rendered output never
    /// depend on dictionary or file-system enumeration order.
    /// </summary>
    public static readonly Comparer<ModuleId> DeterministicOrder = Comparer<ModuleId>.Create(
        (left, right) => left.Origin != right.Origin
            ? left.Origin.CompareTo(right.Origin)
            : string.Compare(left.Name, right.Name, StringComparison.Ordinal));

    private readonly Dictionary<ModuleId, List<ImportEdge>> _edgesByImporter = new();
    private readonly HashSet<ModuleId> _nodes = new();
    private HashSet<ImportEdge>? _cyclicEdges;

    public void AddNode(ModuleId id)
    {
        _nodes.Add(id);
        if (!_edgesByImporter.ContainsKey(id))
        {
            _edgesByImporter[id] = [];
        }
    }

    public void AddEdge(ModuleId importer, ModuleId target, ImportStatementNode? site)
    {
        AddNode(importer);
        AddNode(target);
        _edgesByImporter[importer].Add(new ImportEdge(importer, target, site));
        _cyclicEdges = null; // graph mutated — invalidate the memoized cycle set
    }

    public bool Contains(ModuleId id) => _nodes.Contains(id);

    public IReadOnlyList<ImportEdge> EdgesFrom(ModuleId id) =>
        _edgesByImporter.TryGetValue(id, out var edges) ? edges : [];

    public IReadOnlyList<ModuleId> SortedNodes() => [.. _nodes.OrderBy(id => id, DeterministicOrder)];

    /// <summary>
    /// True when the edge's target can reach back to its importer — the
    /// import participates in a cycle, so no analysis order can provide the
    /// imported interface. Memoized: callers query only after the graph is
    /// fully built.
    /// </summary>
    public bool IsCyclic(ImportEdge edge) => CyclicEdges.Contains(edge);

    private HashSet<ImportEdge> CyclicEdges =>
        _cyclicEdges ??= [.. _edgesByImporter.Values.SelectMany(edges => edges).Where(edge => Reaches(edge.Target, edge.Importer))];

    private bool Reaches(ModuleId from, ModuleId to)
    {
        var visited = new HashSet<ModuleId>();
        var stack = new Stack<ModuleId>();
        stack.Push(from);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current.Equals(to))
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            foreach (var edge in EdgesFrom(current))
            {
                stack.Push(edge.Target);
            }
        }

        return false;
    }

    /// <summary>
    /// The display path of a cyclic import — <c>[importer, target, …,
    /// importer]</c>, each step an import edge — or null when the target
    /// does not reach back to the importer.
    /// </summary>
    public IReadOnlyList<string>? FindCyclePath(ModuleId importer, ModuleId target)
    {
        if (importer.Equals(target))
        {
            return [importer.Name, importer.Name];
        }

        var parents = new Dictionary<ModuleId, ModuleId>();
        var visited = new HashSet<ModuleId> { target };
        var stack = new Stack<ModuleId>();
        stack.Push(target);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            foreach (var edge in EdgesFrom(current))
            {
                if (!visited.Add(edge.Target))
                {
                    continue;
                }

                parents[edge.Target] = current;
                if (edge.Target.Equals(importer))
                {
                    // Walk importer → … → target back through the parents,
                    // then present it in import direction, loop closed.
                    var chain = new List<ModuleId> { importer };
                    var walk = importer;
                    while (!walk.Equals(target))
                    {
                        walk = parents[walk];
                        chain.Add(walk);
                    }

                    chain.Reverse();
                    return [importer.Name, .. chain.Select(id => id.Name)];
                }

                stack.Push(edge.Target);
            }
        }

        return null;
    }

    /// <summary>
    /// Dependencies-first analysis order (Kahn's algorithm over the
    /// non-cyclic edges — cyclic imports never provide an interface, so
    /// excluding them always leaves a DAG). Ties break through
    /// <see cref="DeterministicOrder"/>. Every node appears exactly once.
    /// </summary>
    public IReadOnlyList<ModuleId> AnalysisOrder()
    {
        var pendingImports = SortedNodes().ToDictionary(
            id => id,
            id => EdgesFrom(id).Where(edge => !IsCyclic(edge)).Select(edge => edge.Target).ToList());

        var waitingOn = pendingImports.ToDictionary(entry => entry.Key, entry => entry.Value.Count);
        var importers = new Dictionary<ModuleId, List<ModuleId>>();
        foreach (var (id, targets) in pendingImports)
        {
            foreach (var target in targets)
            {
                if (!importers.TryGetValue(target, out var list))
                {
                    importers[target] = list = [];
                }

                list.Add(id);
            }
        }

        var ready = new SortedSet<ModuleId>(waitingOn.Where(entry => entry.Value == 0).Select(entry => entry.Key), DeterministicOrder);
        var order = new List<ModuleId>();
        while (ready.Count > 0)
        {
            var next = ready.Min;
            ready.Remove(next);
            order.Add(next);
            foreach (var importer in importers.GetValueOrDefault(next, []))
            {
                if (--waitingOn[importer] == 0)
                {
                    ready.Add(importer);
                }
            }
        }

        return order;
    }

    /// <summary>
    /// Every module reachable from <paramref name="start"/> through import
    /// edges (cyclic ones included), start itself included — the set of
    /// modules whose diagnostics gate a run of <paramref name="start"/>.
    /// </summary>
    public IReadOnlySet<ModuleId> ReachableFrom(ModuleId start)
    {
        var closure = new HashSet<ModuleId>();
        var stack = new Stack<ModuleId>();
        stack.Push(start);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!closure.Add(current))
            {
                continue;
            }

            foreach (var edge in EdgesFrom(current))
            {
                stack.Push(edge.Target);
            }
        }

        return closure;
    }
}
