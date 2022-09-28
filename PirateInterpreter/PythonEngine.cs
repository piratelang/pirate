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
            Script = scriptPath + "/output.py";
        }

        public string Script { get; set; }

        public dynamic InvokeMain(string methodName)
        {
            engine = Python.CreateEngine();
            scriptSource = engine.CreateScriptSourceFromFile(Script);
            compiledCode = scriptSource.Compile();
            scope = engine.CreateScope();

            compiledCode.Execute(scope);

            dynamic main = scope.GetVariable("main");

            return main();
        }
    }
}