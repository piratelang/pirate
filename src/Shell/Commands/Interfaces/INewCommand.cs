namespace Shell.Commands.Interfaces;

public interface INewCommand
{
    void Help();
    object Run(string[] arguments);
}
