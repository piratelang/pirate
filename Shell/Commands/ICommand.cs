using Common;

namespace Shell.Commands
{
    public interface ICommand
    {
        string version { get; set; }
        Logger logger { get; set; }

        void Help();
        void Run(string[] arguments);
        void Error(string message);
    }
}