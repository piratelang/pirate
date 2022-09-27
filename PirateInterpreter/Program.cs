using PirateInterpreter;

var pythonEngine = new PythonEngine("C:/MyPorjects/PirateLang/PirateInterpreter/Python/test.txt");

object[] parameters = new object[] {
    new List<string>() {"hello", "world"}
};
var result = pythonEngine.InvokeMethodWithParameters("main", parameters);

if (result != null)
{
	foreach (var item in result)
    {
        Console.WriteLine(item);
    }
}