namespace Shell.Commands.Interfaces;

public interface ICommand
{
    void Error(string message);
    void Help();
    void Run(string[] arguments);
}
