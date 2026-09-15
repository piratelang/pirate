namespace Shell.Commands.Interfaces;

public interface IInitCommand
{
    void Help();
    object Run(string[] arguments);
}
