using Shell.ModuleList;

namespace Shell.Commands.Interfaces;

public interface IBuildCommand : ICommand
{
    bool CheckModuleList(List<Module> moduleList, string file);
}
