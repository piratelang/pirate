namespace Common.Interfaces;

/// <inheritdoc cref="EnvironmentVariables"/>
public interface IEnvironmentVariables
{
    string GetVariable(string variablename);
}