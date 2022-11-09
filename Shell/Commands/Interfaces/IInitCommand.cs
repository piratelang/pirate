namespace Shell.Commands.Interfaces;

public interface IInitCommand
{
    void Help();
    void Run(string[] arguments);
}
