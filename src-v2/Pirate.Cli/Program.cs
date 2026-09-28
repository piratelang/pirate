using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pirate.Cli;
using Pirate.Cli.Commands;
using Pirate.Cli.Services;
using Pirate.Semantics;
using Spectre.Console;
using Spectre.Console.Cli;

try
{
    // Windows consoles don't default to UTF-8, which mangles Spectre's
    // Unicode box-drawing glyphs (Rule, rounded Table borders) into "?"/"�".
    // Redirected/piped output can reject this on some platforms, hence the
    // try/catch — falling back to whatever encoding was already set is fine.
    Console.OutputEncoding = Encoding.UTF8;
}
catch (IOException)
{
}

if (args.Length == 0)
{
    Banner.Render();
    return 0;
}

// Composition root: each project registers its own services through its
// Add*() extension; the CLI adds the seams that glue them together and
// commands receive dependencies via constructor injection (TypeRegistrar
// bridges Spectre.Console.Cli onto this container).
var services = new ServiceCollection();
services.AddPirateSemantics();
services.AddSingleton<ICompilationPipeline, CompilationPipeline>();

var app = new CommandApp(new TypeRegistrar(services));
app.Configure(config =>
{
    config.SetApplicationName("pirate");
    config.SetExceptionHandler((ex, _) =>
    {
        // CommandAppException (parse/validation errors, e.g. NewCommandSettings.Validate())
        // is an expected, user-facing CLI error, not a bug - render its message plainly
        // (using its own Pretty renderable when Spectre attached one) instead of the
        // "unexpected error" + stack-trace treatment reserved for genuine faults below.
        if (ex is CommandAppException commandAppException)
        {
            if (commandAppException.Pretty is { } pretty)
            {
                AnsiConsole.Write(pretty);
            }
            else
            {
                AnsiConsole.MarkupLine($"[{Theme.Error}]Error: {Markup.Escape(commandAppException.Message)}[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[{Theme.Error}]Unexpected error: {Markup.Escape(ex.Message)}[/]");
        }
        return -1;
    });

    config.AddCommand<RunCommand>("run")
        .WithDescription("Run the specified file.")
        .WithExample("run")
        .WithExample("run", "main");

    config.AddCommand<InitCommand>("init")
        .WithDescription("Initializes a new pirate project.")
        .WithExample("init")
        .WithExample("init", "main")
        .WithExample("init", "-n", "my-project");

    config.AddCommand<NewCommand>("new")
        .WithDescription("Creates a new file from a template.")
        .WithExample("new", "list")
        .WithExample("new", "pirate", "main")
        .WithExample("new", "gitignore")
        .WithExample("new", "gitattributes");

    config.AddCommand<BuildCommand>("build")
        .WithDescription("Builds the modules in the current folder.")
        .WithExample("build")
        .WithExample("build", "main");

    config.AddCommand<ShellCommand>("shell")
        .WithDescription("Opens the pirate REPL.");
});

return app.Run(args);
