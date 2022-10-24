namespace Shell.Commands.Interfaces;

public interface IShellCommand
{
    void Help();
    void Run(string[] arguments);
}
