namespace PirateInterpreter.Interpreters.Interfaces;

public interface IInterpreterFactory
{
    BaseInterpreter GetInterpreter(INode node);
}
