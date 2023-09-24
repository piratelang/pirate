namespace PirateLang.Commands.Interfaces;

public interface INewCommand
{
    void Help();
    object Run(string[] arguments);
}
