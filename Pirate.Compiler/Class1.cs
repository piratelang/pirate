using Pirate.Parser;
using Pirate.Parser.Node.Interfaces;

namespace Pirate.Compiler.Interfaces;

public interface ICompiler
{
    CompilationResult Compile(Scope scope, string outputPath, string fileName);
    CompilationResult CompileToAssembly(Scope scope, string outputAssemblyPath);
}

public class CompilationResult
{
    public bool Success { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
