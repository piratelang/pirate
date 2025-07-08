using Pirate.Compiler.Interfaces;
using Pirate.Parser;
using Pirate.Common.Logger.Interfaces;

namespace Pirate.Compiler;

/// <summary>
/// IL-based compiler stub - not fully implemented yet
/// </summary>
public class ILCompiler : ICompiler
{
    private readonly ILogger _logger;

    public ILCompiler(ILogger logger)
    {
        _logger = logger;
    }

    public CompilationResult Compile(Scope scope, string outputPath, string fileName)
    {
        var result = new CompilationResult();
        
        try
        {
            _logger.Info($"IL compilation not yet implemented for {fileName}");
            result.Errors.Add("IL compilation is not yet implemented. Use CSharpTranspiler instead.");
        }
        catch (Exception ex)
        {
            result.Errors.Add($"Compilation failed: {ex.Message}");
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
}