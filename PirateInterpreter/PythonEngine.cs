using Common;
using Common.Enum;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace PirateInterpreter
{
    public class PythonEngine
    {
        private ScriptSource scriptSource;
        private CompiledCode compiledCode;
        private ScriptScope scope;
        private ScriptEngine engine;

        public PythonEngine(string scriptPath)
        {
            Script = scriptPath;
        }

        public string Script { get; set; }

        public dynamic InvokeMain(string methodName)
        {
            Logger.Log("Creating Engine, Script, Code and Scope", this.GetType().Name, LogType.INFO);
            engine = Python.CreateEngine();
            scriptSource = engine.CreateScriptSourceFromFile(Script);
            compiledCode = scriptSource.Compile();
            scope = engine.CreateScope();

            dynamic main;

            try
            {
                compiledCode.Execute(scope);
                main = scope.GetVariable("main");
                Logger.Log("Successfully Executed", this.GetType().Name, LogType.INFO);
                return main();
            }
            catch(Exception exception)
            {
                Logger.Log($"Execute Exception: {exception.ToString()}", this.GetType().Name, LogType.ERROR);
                return null;    
            }
        }
    }
}