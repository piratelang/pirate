using Pirate.Build.Actions.Util;
using Pirate.Common.Interfaces;
using Pirate.Interpreter.Values;

namespace Pirate.Build.Actions;

public class RunAction : IRunAction
{
    private IInterpreter interpreter;
    private readonly IBuildAction buildAction;
    private readonly IObjectSerializer objectSerializer;
    private readonly ILogger logger;

    public RunAction(IInterpreter interpreter, IBuildAction buildAction, IObjectSerializer objectSerializer, ILogger logger)
    {
        this.interpreter = interpreter;
        this.buildAction = buildAction;
        this.objectSerializer = objectSerializer;
        this.logger = logger;
    }

    public List<BaseValue> Execute(ProjectFile projectFile, string path)
    {
        logger.Info($"Running {projectFile.PropertyGroup.ProjectName}");
        if (ValidateProjectFile(projectFile)) return null;

        logger.Info($"Building {projectFile.PropertyGroup.ProjectName}");
        buildAction.Execute(projectFile, path);

        var module = projectFile.ItemGroup.Select(itemGroup => itemGroup.Modules.Select(module => module.EntryPoint == true ? module : null).FirstOrDefault()).FirstOrDefault();
        var fileName = module.File.Replace(".pirate", "").Replace("./", "");
        var scope = CacheUtil.GetScopeFromCache(fileName, objectSerializer, logger, path);

        return interpreter.StartInterpreter(scope);
    }

    private bool ValidateProjectFile(ProjectFile projectFile)
    {
        if (projectFile.ItemGroup == null)
        {
            logger.Error("No modules found");
            return true;
        }

        if (projectFile.ItemGroup.SelectMany(itemGroup => itemGroup.Modules).Count() == 0)
        {
            logger.Error("No modules found");
            return true;
        }

        return false;
    }
}
