using Pirate.Build.Project.Models;

namespace Pirate.Build.Actions.Interfaces;

public interface IAction
{
    public void Execute(ProjectFile projectFile);
}