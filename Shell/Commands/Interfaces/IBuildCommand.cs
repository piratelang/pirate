using Shell.ModuleList;

namespace Shell.Commands.Interfaces;

public interface IBuildCommand
{
    bool CheckModuleList(List<Module> moduleList, string file);
    void Help();
    void Run(string[] arguments);
}
