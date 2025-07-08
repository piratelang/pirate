using System.Text;
using Pirate.Compiler.Interfaces;
using Pirate.Parser;
using Pirate.Parser.Node;
using Pirate.Parser.Node.Interfaces;
using Pirate.Common.Logger.Interfaces;

namespace Pirate.Compiler;

/// <summary>
/// Simple transpiler that converts Pirate AST to C# code
/// This is a basic implementation to demonstrate compilation capability
/// </summary>
public class CSharpTranspiler : ICompiler
{
    private readonly ILogger _logger;
    private readonly StringBuilder _output;
    private int _indentLevel;

    public CSharpTranspiler(ILogger logger)
    {
        _logger = logger;
        _output = new StringBuilder();
        _indentLevel = 0;
    }

    public CompilationResult Compile(Scope scope, string outputPath, string fileName)
    {
        var result = new CompilationResult();
        
        try
        {
            _logger.Info($"Starting transpilation of {fileName}");
            
            // Reset output
            _output.Clear();
            _indentLevel = 0;
            
            // Generate C# code
            GenerateCSharpCode(scope, fileName);
            
            // Write to file
            var csharpFilePath = Path.Combine(outputPath, $"{fileName}.cs");
            File.WriteAllText(csharpFilePath, _output.ToString());
            
            result.Success = true;
            result.OutputPath = csharpFilePath;
            _logger.Info($"Transpilation successful: {csharpFilePath}");
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Transpilation failed: {ex.Message}");
            _logger.Error(ex);
        }
        
        return result;
    }

    public CompilationResult CompileToAssembly(Scope scope, string outputAssemblyPath)
    {
        var fileName = Path.GetFileNameWithoutExtension(outputAssemblyPath);
        var outputPath = Path.GetDirectoryName(outputAssemblyPath) ?? "./";
        return Compile(scope, outputPath, fileName);
    }

    private void GenerateCSharpCode(Scope scope, string programName)
    {
        // Generate using statements
        WriteLine("using System;");
        WriteLine("using System.Collections.Generic;");
        WriteLine();
        
        // Generate namespace and class
        WriteLine($"namespace {programName}");
        WriteLine("{");
        Indent();
        
        WriteLine("public class Program");
        WriteLine("{");
        Indent();
        
        // Generate all functions/statements
        foreach (var node in scope.Nodes)
        {
            GenerateStatement(node);
            WriteLine();
        }
        
        Unindent();
        WriteLine("}");
        Unindent();
        WriteLine("}");
    }

    private void GenerateStatement(INode node)
    {
        switch (node)
        {
            case FunctionDeclarationNode funcNode:
                GenerateFunction(funcNode);
                break;
                
            case VariableDeclarationNode varNode:
                GenerateVariableDeclaration(varNode);
                break;
                
            case FunctionCallNode callNode:
                GenerateFunctionCall(callNode);
                WriteLine(";");
                break;
                
            case CommentNode commentNode:
                WriteLine($"// Comment node");
                break;
                
            default:
                WriteLine($"// Unhandled node type: {node.GetType().Name}");
                break;
        }
    }

    private void GenerateFunction(FunctionDeclarationNode funcNode)
    {
        var functionName = funcNode.Identifier.Value?.Value?.ToString() ?? "unknown";
        var returnType = GetCSharpType(funcNode.ReturnType?.Value?.ToString() ?? "void");
        
        // Generate simple method signature
        WriteLine($"public static {returnType} {functionName}()");
        WriteLine("{");
        Indent();
        
        // Generate function body - simplified for now
        foreach (var statement in funcNode.Statements)
        {
            GenerateStatement(statement);
        }
        
        // Add default return if needed
        if (returnType != "void")
        {
            WriteLine($"return default({returnType});");
        }
        
        Unindent();
        WriteLine("}");
    }

    private void GenerateVariableDeclaration(VariableDeclarationNode varNode)
    {
        var varName = varNode.Identifier?.Value?.Value?.ToString() ?? "unknown";
        var varType = GetCSharpType(varNode.TypeToken?.Value?.ToString() ?? "var");
        
        Write($"{varType} {varName}");
        
        if (varNode.Value != null)
        {
            Write(" = ");
            GenerateExpression(varNode.Value);
        }
        
        WriteLine(";");
    }

    private void GenerateFunctionCall(FunctionCallNode callNode)
    {
        var functionName = callNode.Identifier?.Value?.Value?.ToString() ?? "unknown";
        
        // Handle special cases
        if (functionName == "print")
        {
            Write("Console.WriteLine");
        }
        else
        {
            Write(functionName);
        }
        
        Write("()"); // Simplified - no arguments for now
    }

    private void GenerateExpression(INode expression)
    {
        switch (expression)
        {
            case ValueNode valueNode:
                GenerateValue(valueNode);
                break;
                
            case FunctionCallNode callNode:
                GenerateFunctionCall(callNode);
                break;
                
            default:
                Write($"/* Unhandled expression: {expression.GetType().Name} */");
                break;
        }
    }

    private void GenerateValue(ValueNode valueNode)
    {
        var token = valueNode.Value;
        var value = token?.Value?.ToString() ?? "";
        
        // Simple value generation based on token type
        switch (token?.TokenType.ToString()?.ToUpper())
        {
            case "STRING":
                Write($"\"{value}\"");
                break;
                
            case "INT":
                Write(value);
                break;
                
            case "FLOAT":
                Write($"{value}f");
                break;
                
            case "BOOLEAN":
                Write(value.ToLower());
                break;
                
            case "IDENTIFIER":
                Write(value);
                break;
                
            default:
                Write(value);
                break;
        }
    }

    private string GetCSharpType(string pirateType)
    {
        return pirateType?.ToLower() switch
        {
            "int" => "int",
            "string" => "string",
            "float" => "float",
            "char" => "char",
            "bool" => "bool",
            "void" => "void",
            "var" => "var",
            _ => "object"
        };
    }

    private void WriteLine(string text = "")
    {
        if (!string.IsNullOrEmpty(text))
        {
            _output.AppendLine(new string(' ', _indentLevel * 4) + text);
        }
        else
        {
            _output.AppendLine();
        }
    }

    private void Write(string text)
    {
        if (_output.Length == 0 || _output[_output.Length - 1] == '\n')
        {
            _output.Append(new string(' ', _indentLevel * 4));
        }
        _output.Append(text);
    }

    private void Indent()
    {
        _indentLevel++;
    }

    private void Unindent()
    {
        if (_indentLevel > 0)
            _indentLevel--;
    }
}