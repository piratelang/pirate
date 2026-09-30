using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics;

/// <summary>
/// The set of standard-library functions an <c>extern</c> declaration may
/// import, with their static signatures.
///
/// This table is the single source of truth for builtin signatures; when
/// <c>Pirate.StandardLibrary</c> (VM phase) lands, its native
/// implementations register by the same dotted names and
/// <see cref="Entry.Index"/> becomes the <c>CALL_BUILTIN</c> operand — a
/// unit test there must assert registry↔implementation coverage.
///
/// <c>Standard.String.Concat</c> takes exactly two strings; variadic
/// builtins are a follow-up issue.
/// </summary>
public static class BuiltinRegistry
{
    /// <summary>
    /// A builtin's static signature and its index in the builtin table.
    /// </summary>
    public sealed record Entry(string Name, int Index, IReadOnlyList<PirateType> Parameters, PirateType ReturnType);

    private static readonly List<Entry> Entries =
    [
        new Entry("Standard.Terminal.Print", 0, [PirateType.String], PirateType.Void),
        new Entry("Standard.Terminal.PrintLine", 1, [PirateType.String], PirateType.Void),
        new Entry("Standard.Terminal.Read", 2, [], PirateType.String),
        new Entry("Standard.String.Length", 3, [PirateType.String], PirateType.Int),
        new Entry("Standard.String.CharAt", 4, [PirateType.String, PirateType.Int], PirateType.Char),
        new Entry("Standard.String.Concat", 5, [PirateType.String, PirateType.String], PirateType.String),
        new Entry("Standard.String.Split", 6, [PirateType.String, PirateType.String], PirateType.ArrayOf(ScalarType.String)),
        new Entry("Standard.String.IndexOf", 7, [PirateType.String, PirateType.String], PirateType.Int),
        new Entry("Standard.String.LastIndexOf", 8, [PirateType.String, PirateType.String], PirateType.Int),
        new Entry("Standard.String.CharCodeAt", 9, [PirateType.String, PirateType.Int], PirateType.Int),
    ];

    private static readonly Dictionary<string, Entry> Map =
        Entries.ToDictionary(e => e.Name, StringComparer.Ordinal);

    public static IReadOnlyList<Entry> All => new ReadOnlyCollection<Entry>(Entries);

    /// <summary>
    /// Looks up a dotted path such as <c>Standard.Terminal.Print</c>; null
    /// when no such builtin exists.
    /// </summary>
    public static Entry? Lookup(string dottedName) =>
        Map.TryGetValue(dottedName, out var entry) ? entry : null;

    /// <summary>
    /// The builtins under one namespace group, e.g. everything starting
    /// <c>Standard.Terminal.</c> — the set <c>import standard Terminal;</c>
    /// binds. Empty for unknown namespaces.
    /// </summary>
    public static IReadOnlyList<Entry> InNamespace(string dottedNamespace)
    {
        var prefix = dottedNamespace + ".";
        return Entries.Where(entry => entry.Name.StartsWith(prefix, StringComparison.Ordinal)).ToList();
    }
}
