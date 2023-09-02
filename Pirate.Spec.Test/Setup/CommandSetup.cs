﻿using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Lexer;
using Pirate.Lexer.Interfaces;
using Pirate.Lexer.Tokens;
using Pirate.Parser;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;

namespace Pirate.Spec.Test.Setup;

public class CommandSetup
{    
    private readonly ScenarioContext _scenarioContext;
    
    private readonly FileWriteHandler _fileWriteHandler;
    private readonly FileReadHandler _fileReadHandler;
    
    private readonly ILogger _logger;
    private readonly EnvironmentVariables _environmentVariables;
    private readonly IObjectSerializer _objectSerializer;
    
    public CommandSetup(ScenarioContext scenarioContext, FileWriteHandler fileWriteHandler, FileReadHandler fileReadHandler)
    {
        _scenarioContext = scenarioContext;
        _fileWriteHandler = fileWriteHandler;
        _fileReadHandler = fileReadHandler;
        
        _logger = new Logger();
        _environmentVariables = new EnvironmentVariables(_fileReadHandler, fileWriteHandler);
        _objectSerializer = new ObjectSerializer(_logger, _environmentVariables, fileWriteHandler, fileReadHandler);
    }
    
    public RunCommand GetRunCommand()
    {
        ILexer lexer = new Lexer.Lexer(_logger, new TokenRepository(new KeyWordService()));
        IParser parser = new Parser.Parser(_logger, _objectSerializer);
        IInterpreterFactory interpreterFactory = new InterpreterFactory(new StandardLibraryCallManager(_logger), _logger);
        IInterpreter interpreter = new Interpreter.Interpreter(_objectSerializer, _logger, interpreterFactory);
        IModuleListRepository moduleListRepository = new ModuleListRepository(_logger, _fileWriteHandler, _fileReadHandler);

        IBuildCommand buildCommand = new BuildCommand(_logger, _objectSerializer, parser, lexer, moduleListRepository,
            _fileReadHandler, _environmentVariables);
        return new RunCommand(_logger, _objectSerializer, buildCommand, interpreter, _fileReadHandler,
            _environmentVariables);
    }
}