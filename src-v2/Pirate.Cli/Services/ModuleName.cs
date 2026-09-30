namespace Pirate.Cli.Services;

/// <summary>
/// Module/type names as the language spells them (docs/GRAMMAR.md §4.4): the
/// root namespace (the <c>.fleet</c> file's base name) followed by the
/// folder path relative to the project root, then the file's type name —
/// dots throughout, directory separators replaced and the extension
/// stripped (<c>lib/core.pirate</c> under root namespace <c>shop</c> →
/// <c>shop.lib.core</c>). This is exactly the form
/// <c>import module shop.lib.core;</c> uses.
/// </summary>
public static class ModuleName
{
    public static string FromRelativePath(string rootNamespace, string relativePath)
    {
        var withoutExtension = Path.ChangeExtension(relativePath, null);
        var dotted = withoutExtension.Replace('/', '.').Replace('\\', '.');
        return $"{rootNamespace}.{dotted}";
    }

    public static string Of(string rootNamespace, string rootDirectory, string filePath) =>
        FromRelativePath(rootNamespace, Path.GetRelativePath(rootDirectory, filePath));
}
