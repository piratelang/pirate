using Pirate.Build.Actions.Util;
using Pirate.Build.Models;
using Pirate.Build.Project.Models;
using Pirate.Common;
using Pirate.Common.Interfaces;
using Pirate.Parser;

namespace Pirate.Build.Actions;

public class BuildAction : IBuildAction
{
    private Lexer.Lexer lexer;
    private IParser parser;
    private IInterpreter interpreter;

    private readonly ILogger logger;
    private readonly IFileReadHandler fileReadHandler;
    private readonly IObjectSerializer objectSerializer;

    public BuildAction(
        Lexer.Lexer lexer, 
        IParser parser, 
        IInterpreter interpreter, 
        ILogger logger,
        IFileReadHandler fileReadHandler,
        IObjectSerializer objectSerializer
    )
    {
        this.lexer = lexer;
        this.parser = parser;
        this.interpreter = interpreter;
        this.logger = logger;
        this.fileReadHandler = fileReadHandler;
        this.objectSerializer = objectSerializer;
    }

    public void Execute(ProjectFile projectFile, string path)
    {
        logger.Info($"Building {projectFile.PropertyGroup.ProjectName}");
        if (ProjectUtil.ValidateProjectFile(projectFile, logger)) return;

        foreach (var module in projectFile.ItemGroup.SelectMany(itemGroup => itemGroup.Modules))
        {
            var fileName = module.File.Replace(".pirate", "").Replace("./", "");
            var text = fileReadHandler.ReadAllTextFromFile(fileName, FileExtension.PIRATE, path).Result;
            if (text == null) throw new Exception($"{fileName} contains no text");

            // Running Lexer
            logger.Info($"Lexing {module.File}\n");
            var tokens = lexer.MakeTokens(text, "test").ToList();
            if (tokens.Count() == 0) throw new Exception($"Error occured while lexing tokens, in the file {fileName}.");

            // Running Parser
            logger.Info($"Parsing {module.File}\n");
            var ast = parser.StartParse(tokens, fileName);
            if (ast == null) throw new Exception($"Error occured while parsing tokens, in the file {fileName}.");

            CacheUtil.AddScopeToCache(ast, fileName, objectSerializer, logger, path);
        }
    }
}
