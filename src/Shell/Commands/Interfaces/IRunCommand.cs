namespace Shell.Commands.Interfaces;

public interface IRunCommand
{
    void Help();
    object Run(string[] arguments);
}
