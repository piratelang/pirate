using Microsoft.Extensions.DependencyInjection;
using Shell;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;
using Pirate.Common.Interfaces;
using Pirate.Common.FileHandlers;
using Pirate.Common;
using Pirate.Common.FileHandlers.Interfaces;
using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.StandardLibrary.Interfaces;
using Pirate.Interpreter;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Parser;
using Pirate.Lexer.Tokens;
using Pirate.Lexer;
using Pirate.Lexer.Interfaces;
using Pirate.Lexer.Tokens.Interfaces;

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
builder.AddTransient<IStandardLibraryCallManager, StandardLibraryCallManager>();

var provider = builder.BuildServiceProvider();
var app = provider.GetRequiredService<Application>();

app.Run(args, version);