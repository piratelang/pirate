using Xunit;
using FakeItEasy;
using Pirate.Compiler;
using Pirate.Compiler.Interfaces;
using Pirate.Parser;
using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;
using Pirate.Common.Logger.Interfaces;
using Pirate.Lexer.Tokens;

namespace Pirate.Compiler.Test;

public class CSharpTranspilerTest
{
    [Fact]
    public void ShouldCompileSimpleFunction()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var transpiler = new CSharpTranspiler(logger);
        
        var scope = CreateSimpleScope();
        var outputPath = "./test_output";
        var fileName = "test";
        
        // Create output directory
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        // Act
        var result = transpiler.Compile(scope, outputPath, fileName);
        
        // Assert
        Assert.True(result.Success);
        Assert.Contains("test.cs", result.OutputPath);
        Assert.True(File.Exists(result.OutputPath));
        
        var generatedCode = File.ReadAllText(result.OutputPath);
        Assert.Contains("namespace test", generatedCode);
        Assert.Contains("public class Program", generatedCode);
        Assert.Contains("public static void main()", generatedCode);
        
        // Clean up
        if (File.Exists(result.OutputPath))
            File.Delete(result.OutputPath);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath);
    }
    
    [Fact]
    public void ShouldHandleVariableDeclaration()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var transpiler = new CSharpTranspiler(logger);
        
        var scope = CreateScopeWithVariable();
        var outputPath = "./test_output";
        var fileName = "test_var";
        
        // Create output directory
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);
        
        // Act
        var result = transpiler.Compile(scope, outputPath, fileName);
        
        // Assert
        Assert.True(result.Success);
        
        var generatedCode = File.ReadAllText(result.OutputPath);
        Assert.Contains("var message", generatedCode);
        
        // Clean up
        if (File.Exists(result.OutputPath))
            File.Delete(result.OutputPath);
        if (Directory.Exists(outputPath))
            Directory.Delete(outputPath);
    }
    
    [Fact]
    public void ShouldHandleCompilationErrors()
    {
        // Arrange
        var logger = A.Fake<ILogger>();
        var transpiler = new CSharpTranspiler(logger);
        
        var scope = new Scope(logger); // Empty scope
        var outputPath = "/invalid/path/that/does/not/exist";
        var fileName = "test";
        
        // Act
        var result = transpiler.Compile(scope, outputPath, fileName);
        
        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.Errors);
    }
    
    private Scope CreateSimpleScope()
    {
        var logger = A.Fake<ILogger>();
        var scope = new Scope(logger);
        
        // Create a simple main function
        var mainIdentifier = CreateValueNode("main");
        var returnType = CreateToken("void");
        var statements = new List<INode>();
        
        var funcNode = A.Fake<FunctionDeclarationNode>();
        A.CallTo(() => funcNode.Identifier).Returns(mainIdentifier);
        A.CallTo(() => funcNode.ReturnType).Returns(returnType);
        A.CallTo(() => funcNode.Statements).Returns(statements);
        A.CallTo(() => funcNode.Parameters).Returns(new List<IParameterDefinitionNode>());
        
        scope.AddNode(funcNode);
        return scope;
    }
    
    private Scope CreateScopeWithVariable()
    {
        var logger = A.Fake<ILogger>();
        var scope = new Scope(logger);
        
        // Create a variable declaration
        var varIdentifier = CreateValueNode("message");
        var typeToken = CreateToken("var");
        var value = CreateValueNode("Hello");
        
        var varNode = A.Fake<VariableDeclarationNode>();
        A.CallTo(() => varNode.Identifier).Returns(varIdentifier);
        A.CallTo(() => varNode.TypeToken).Returns(typeToken);
        A.CallTo(() => varNode.Value).Returns(value);
        
        scope.AddNode(varNode);
        return scope;
    }
    
    private IValueNode CreateValueNode(string value)
    {
        var token = CreateToken(value);
        var valueNode = A.Fake<IValueNode>();
        A.CallTo(() => valueNode.Value).Returns(token);
        return valueNode;
    }
    
    private Token CreateToken(string value)
    {
        var token = A.Fake<Token>();
        A.CallTo(() => token.Value).Returns(value);
        A.CallTo(() => token.ToString()).Returns(value);
        return token;
    }
}