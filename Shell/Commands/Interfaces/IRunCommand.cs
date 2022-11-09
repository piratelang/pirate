namespace Shell.Commands.Interfaces;

public interface IRunCommand
{
    void Help();
    void Run(string[] arguments);
}
