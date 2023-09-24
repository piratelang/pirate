namespace PirateLang.Commands.Interfaces;

public interface ICommand
{
    void Error(string message);
    void Help();
    object Run(string[] arguments);
}
