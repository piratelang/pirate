using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Lexer;
using Pirate.Parser.Interfaces;
using PirateLang.Commands;
using PirateLang.Commands.Interfaces;
using Shell.ModuleList;
using Shell.ModuleList.interfaces;

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
        //Lexer.Lexer lexer = new Lexer.Lexer(_logger, new TokenRepository(new KeyWordService()));
        //IParser parser = new Parser.Parser(_logger, _objectSerializer);
        //Runtime runtime = new Runtime(_logger);
        //IInterpreterFactory interpreterFactory = new InterpreterFactory(new StandardLibraryProvider(_logger), _logger, runtime);
        //IInterpreter interpreter = new Interpreter.Interpreter(_objectSerializer, _logger, interpreterFactory);
        //IModuleListRepository moduleListRepository = new ModuleListRepository(_logger, _fileWriteHandler, _fileReadHandler);

        //IBuildCommand buildCommand = new IBuildCommand(_logger, _objectSerializer, parser, lexer, moduleListRepository,
        //    _fileReadHandler, _environmentVariables);
        //return new RunCommand(_logger, _objectSerializer, buildCommand, interpreter, _fileReadHandler,
        //    _environmentVariables);
        return null;
    }

    public ILogger GetLogger()
    {
        return _logger;
    }
}