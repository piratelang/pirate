using Common;
using PirateInterpreter;
using PirateLexer;
using Microsoft.Extensions.DependencyInjection;
using PirateParser;
using Shell;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;

var version = "1.0.0";
var location = $"bin/pirate{version}";

var builder = new ServiceCollection();
builder.AddSingleton<Application, Application>();
builder.AddSingleton<IObjectSerializer,ObjectSerializer>();
builder.AddSingleton<ILogger, Logger>();
builder.AddSingleton<IModuleListRepository, ModuleListRepository>();
builder.AddSingleton<ICommandManager, CommandManager>();
builder.AddSingleton<CommandFactory, CommandFactory>();

builder.AddTransient<IBuildCommand, BuildCommand>();
builder.AddTransient<IInitCommand, InitCommand>();
builder.AddTransient<INewCommand, NewCommand>();
builder.AddTransient<IRunCommand, RunCommand>();
builder.AddTransient<IShellCommand, ShellCommand>();
builder.AddTransient<ICommandFactory, CommandFactory>();

builder.AddTransient<ILexer, Lexer>();
builder.AddTransient<IParser, Parser>();
builder.AddTransient<IInterpreter, Interpreter>();

var provider = builder.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();

app.Run(args, version, location);