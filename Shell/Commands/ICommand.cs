namespace Shell.Commands
{
    public interface ICommand
    {
        string version { get; set; }

        void Help();
        void Run(string[] arguments);
    }
}