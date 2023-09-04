using Pirate.Interpreter.Interfaces;
using Pirate.Interpreter.Interpreters;
using Pirate.Interpreter.Interpreters.Interfaces;
using Pirate.Interpreter.StandardLibrary;
using Pirate.Interpreter.Values;
using Pirate.Lexer;
using Pirate.Lexer.Tokens;
using Pirate.Parser;
using Pirate.Spec.Test.Setup;
using Shell.Commands;
using Shell.Commands.Interfaces;
using Shell.ModuleList;

namespace Pirate.Spec.Test.StepDefinitions;

[Binding]
public sealed class CommonSteps
{   
    private readonly ScenarioContext _scenarioContext;
    
    private readonly FileWriteHandler _fileWriteHandler;
    private readonly FileReadHandler _fileReadHandler;
    
    private readonly CommandSetup _commandSetup;
    
    public CommonSteps(ScenarioContext scenarioContext, FileWriteHandler fileWriteHandler, FileReadHandler fileReadHandler, CommandSetup commandSetup)
    {
        _scenarioContext = scenarioContext;
        _fileWriteHandler = fileWriteHandler;
        _fileReadHandler = fileReadHandler;
        
        _commandSetup = commandSetup;
    }
    
    [Given(@"the following pirate code:")]
    public void GivenTheFollowingPirateCode(string pirateCode)
    {
        var fileName = Guid.NewGuid().ToString();
        _scenarioContext.Set(fileName, "FileName");

        _fileWriteHandler.WriteToFile(new FileWriteModel(fileName, FileExtension.PIRATE, "", pirateCode));
        _scenarioContext.Set(pirateCode, "PirateCode");
    }

    [When(@"the code is executed")]
    public void WhenTheCodeIsExecuted()
    {
        // Arrange
        var runCommand = _commandSetup.GetRunCommand();
        var args = new string[] { "run", _scenarioContext.Get<string>("FileName") };
        
        // Act
        var result = runCommand.Run(args);
        
        // Assert
        result.Should().BeOfType(typeof(List<BaseValue>));
        _scenarioContext.Set(result, "CodeResult");
    }

    [AfterScenario]
    public void AfterScenario()
    {
        var fileName = _scenarioContext.Get<string>("FileName");
        // File.Delete(fileName + ".pirate");
    }
}