using Common;
using PirateInterpreter;
using PirateLexer;
using Microsoft.Extensions.DependencyInjection;
using PirateParser;
using Shell;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;
using PirateLexer.Interfaces;
using PirateLexer.Tokens;
using PirateInterpreter.Interfaces;
using PirateInterpreter.Interpreters.Interfaces;
using PirateInterpreter.StandardLibrary.Interfaces;
using PirateInterpreter.Interpreters;
using PirateInterpreter.StandardLibrary;

var version = "1.0.0";

var builder = new ServiceCollection();
builder.AddSingleton<Application, Application>();
builder.AddSingleton<IObjectSerializer,ObjectSerializer>();
builder.AddSingleton<ILogger, Logger>();
builder.AddSingleton<IEnvironmentVariables, EnvironmentVariables>();
builder.AddSingleton<IFileWriteHandler, FileWriteHandler>();
builder.AddSingleton<IFileReadHandler, FileReadHandler>();

//Shell
builder.AddSingleton<IModuleListRepository, ModuleListRepository>();
builder.AddSingleton<ICommandManager, CommandManager>();
builder.AddSingleton<CommandFactory, CommandFactory>();

builder.AddTransient<IBuildCommand, BuildCommand>();
builder.AddTransient<IInitCommand, InitCommand>();
builder.AddTransient<INewCommand, NewCommand>();
builder.AddTransient<IRunCommand, RunCommand>();
builder.AddTransient<IShellCommand, ShellCommand>();
builder.AddTransient<ICommandFactory, CommandFactory>();

//Lexer
builder.AddTransient<ILexer, Lexer>();
builder.AddSingleton<IKeyWordService, KeyWordService>();
builder.AddTransient<ITokenRepository, TokenRepository>();

//Parser
builder.AddTransient<IParser, Parser>();

//Interpreter
builder.AddTransient<IInterpreter, Interpreter>();
builder.AddTransient<IInterpreterFactory, InterpreterFactory>();
builder.AddTransient<IStandardLibraryFactory, StandardLibraryFactory>();

var provider = builder.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();

app.Run(args, version);