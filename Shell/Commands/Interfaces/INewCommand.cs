namespace Shell.Commands.Interfaces;

public interface INewCommand
{
    void Help();
    void Run(string[] arguments);
}
