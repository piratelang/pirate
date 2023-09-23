namespace Shell.Commands.Interfaces;

public interface IShellCommand
{
    void Help();
    object Run(string[] arguments);
}
