using System.Text;
using Pirate.Syntax;
using Spectre.Console;

namespace Pirate.Cli.Services;

/// <summary>
/// Renders <see cref="CompilationError"/> instances as formatted console output
/// with source file excerpts and caret highlighting.
/// <para>
/// Output format:
/// <c>main.pirate:4:12 Expected ';' after expression *SYN-005*</c>
/// <c>  3 | func main() : void {</c>
/// <c>  4 |     var x = 5</c>
/// <c>    |              ^</c>
/// <c>  5 | }</c>
/// </para>
/// </summary>
public static class DiagnosticRenderer
{
    private const int ContextLines = 2;

    /// <summary>
    /// Renders a single error to the console with a source excerpt.
    /// </summary>
    public static void RenderError(string sourceFile, CompilationError error)
    {
        var code = ErrorMapper.Map(error);
        var header = $"{sourceFile}:{error.StartLocation.Line}:{error.StartLocation.Column} {error.Message} *{code}*";
        AnsiConsole.MarkupLine($"[{Theme.Error}]{Markup.Escape(header)}[/]");

        if (!File.Exists(sourceFile)) return;

        var lines = File.ReadAllLines(sourceFile);
        var startLine = Math.Max(1, error.StartLocation.Line - ContextLines);
        var endLine = Math.Min(lines.Length, (error.EndLocation?.Line ?? error.StartLocation.Line) + ContextLines);

        RenderExcerpt(lines, error.StartLocation, error.EndLocation, startLine, endLine);
    }

    /// <summary>
    /// Renders multiple errors for a single source file, deduplicating
    /// the source excerpt when errors share overlapping line ranges.
    /// </summary>
    public static void RenderErrors(string sourceFile, IReadOnlyList<CompilationError> errors)
    {
        foreach (var error in errors)
        {
            RenderError(sourceFile, error);
            AnsiConsole.WriteLine();
        }
    }

    private static void RenderExcerpt(string[] lines, SourceLocation start, SourceLocation? endLocation, int startLine, int endLine)
    {
        var lineNumberWidth = endLine.ToString().Length;

        for (var i = startLine; i <= endLine; i++)
        {
            var lineIndex = i - 1;
            if (lineIndex < 0 || lineIndex >= lines.Length) continue;

            var lineText = lines[lineIndex];
            var linePrefix = $"  {i.ToString().PadLeft(lineNumberWidth)} |";

            AnsiConsole.MarkupLine($"[{Theme.Info}]{Markup.Escape(linePrefix)} {Markup.Escape(lineText)}[/]");

            if (i == start.Line)
            {
                // Source text starts at column lineNumberWidth + 5 (prefix is
                // lineNumberWidth + 4 chars, plus one space), so the caret
                // indent for zero-based col is lineNumberWidth + 5 + col.
                var col = Math.Min(start.Column - 1, lineText.Length);
                var caretEnd = endLocation is { } end && end.Line == start.Line
                    ? Math.Min(end.Column, lineText.Length)
                    : col + 1;
                var caretLength = Math.Max(1, caretEnd - col);

                var caret = new string('^', caretLength);
                AnsiConsole.MarkupLine($"[{Theme.Error}]{new string(' ', lineNumberWidth + 5 + col)}{caret}[/]");
            }
        }
    }
}
