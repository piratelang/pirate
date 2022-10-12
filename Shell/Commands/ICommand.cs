using Common;

namespace Shell.Commands
{
    public interface ICommand
    {
        string version { get; set; }
        Logger logger { get; set; }

        void Help();
        virtual void Run(string[] arguments)
        {

        }
        void Error(string message);
    }
    public abstract class Command
    {
        public string version { get; set; }
        public Logger logger { get; set; }

        public abstract void Help();
        public virtual void Run(string[] arguments)
        {

        }
        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}