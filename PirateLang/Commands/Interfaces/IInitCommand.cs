namespace PirateLang.Commands.Interfaces;

public interface IInitCommand
{
    void Help();
    object Run(string[] arguments);
}
