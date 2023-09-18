namespace Pirate.Build.Actions.Util;

public static class ProjectUtil
{
    public static bool ValidateProjectFile(ProjectFile projectFile, ILogger logger)
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
