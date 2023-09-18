using Pirate.Common;
using Pirate.Common.FileHandler;
using Pirate.Common.Logger;
using Pirate.Interpreter;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Lexer;
using Pirate.Parser;
using Shell.Project;


var logger = new Logger();
var fileReadHandler = new FileReadHandler();
var fileWriteHandler = new FileWriteHandler();
var objectSerializer = new ObjectSerializer(logger, new EnvironmentVariables(fileReadHandler, fileWriteHandler), fileWriteHandler, fileReadHandler);

var lexer = new Lexer(logger, new TokenRepository(new KeyWordService()));
var parser = new Parser(logger);
var interpreter = new Interpreter(logger, new InterpreterFactory(new StandardLibraryProvider(logger), logger, new Runtime(logger)));

var projectFileHandler = new ProjectFileHandler(fileReadHandler, fileWriteHandler);

var runAction = new RunAction(interpreter, new BuildAction(lexer, parser, interpreter, logger, fileReadHandler, objectSerializer), objectSerializer, logger);


runAction.Execute(projectFileHandler.ReadProjectFile("full", "Test").Result, "./Test/");
