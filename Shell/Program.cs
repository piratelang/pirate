using Microsoft.Extensions.DependencyInjection;
using Shell;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;
using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Parser;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;
using Pirate.Interpreter.Runtime;
using PirateLang;
using Pirate.Interpreter.StandarLibrary;

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
builder.AddTransient<Pirate.Lexer.Lexer, Lexer>();
builder.AddSingleton<KeyWordService, KeyWordService>();
builder.AddTransient<TokenRepository, TokenRepository>();

//Parser
builder.AddTransient<IParser, Parser>();

//Interpreter
builder.AddTransient<IInterpreter, Interpreter>();
builder.AddTransient<IInterpreterFactory, InterpreterFactory>();

// Interpreter.Runtime
builder.AddSingleton<IRuntime, Runtime>();

// Interpreter.StandardLibrary
builder.AddTransient<IStandardLibraryProvider, StandardLibraryProvider>();


var provider = builder.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();

app.Run(args, version);