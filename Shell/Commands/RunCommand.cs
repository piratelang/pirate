using Common;
using Common.Enum;
using Interpreter;

namespace Shell.Commands
{
    public class RunCommand : Command
    {
        public ObjectSerializer ObjectSerializer { get; set; }
        public string Location { get; set; }
        public RunCommand(string version, ILogger logger, ObjectSerializer objectSerializer, string location) : base(version, logger)
        {
            ObjectSerializer = objectSerializer;
            Location = location;
        }
        public override void Run(string[] arguments)
        {
            Logger.Log("Starting Run Command", this.GetType().Name, LogType.INFO);
            var fileArgument = "main";
            if (arguments.Length >= 2) { fileArgument = arguments[1]; }
            var fileName = fileArgument.Replace(".pirate", "");

            if (!File.Exists($"./{fileName}.pirate")) Error($"File \"{fileArgument}\" not provided or does not exist.");

            Logger.Log("Starting build", this.GetType().Name, LogType.INFO);

            var buildCommand = new BuildCommand(Version, Logger, ObjectSerializer, Location);
            buildCommand.Run(arguments);
                        
            Logger.Log("Completed Build", this.GetType().Name, LogType.INFO);

            var location = $"bin/pirate{Version}";
            Logger.Log($"Executing {fileName}.pirate\n", this.GetType().Name, LogType.INFO);
            
            var interpreter = new Interpreter.Interpreter(fileName, ObjectSerializer, Logger);
            var interpreterResult = interpreter.StartInterpreter();
            foreach (var item in interpreterResult)
            {
                Console.WriteLine(item.Value);
            }
        }

        public override void Help()
        {
            Console.WriteLine(String.Join(
                Environment.NewLine,
                "Description",
                "   pirate run command",
                "\nUsage",
                "   pirate run [filename]",
                "\nOptions",
                "   -h --help   Show command line help."
            ));
        }
    }
}