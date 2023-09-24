using Shell.ModuleList;

namespace PirateLang.Commands.Interfaces;

public interface IBuildCommand
{
    void Help();
    object Run(string[] arguments);
}
