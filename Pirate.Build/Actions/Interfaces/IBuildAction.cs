namespace Pirate.Build.Actions.Interfaces;

public interface IBuildAction
{
    void Execute(ProjectFile projectFile, string path);
}