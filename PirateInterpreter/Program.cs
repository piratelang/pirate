using PirateInterpreter;

var pythonEngine = new PythonEngine("C:/MyPorjects/PirateLang/PirateInterpreter/Python/test.txt");

var result = pythonEngine.InvokeMain("main");

if (result != null)
{
	foreach (var item in result)
    {
        Console.WriteLine(item);
    }
}

