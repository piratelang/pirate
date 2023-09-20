using Pirate.Build.Project.Models;
using Pirate.Interpreter.Values;

namespace Pirate.Build.Actions.Interfaces;

public interface IRunAction
{
    List<BaseValue> Execute(ProjectFile projectFile, string path);
}
