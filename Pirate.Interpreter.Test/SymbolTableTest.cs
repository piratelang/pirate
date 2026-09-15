using System;
using Pirate.Interpreter.Runtime;
using Pirate.Interpreter.Values;
using Pirate.Interpreter.Values.Interfaces;

namespace Pirate.Interpreter.Test;

public class SymbolTableTest
{
    [Fact]
    public void ShouldSetAndGetValue()
    {
        var logger = A.Fake<ILogger>();
        var valueTable = new ValueTable<IValue>(logger);
        var value = new StringValue("test", logger);

        var setResult = valueTable.Set("test", value);
        var getResult = valueTable.Get("test");

        Assert.True(setResult);
        Assert.Equal(value, getResult);
    }

    [Fact]
    public void ShouldRemoveValue()
    {
        var logger = A.Fake<ILogger>();
        var valueTable = new ValueTable<IValue>(logger);
        valueTable.Set("test", new StringValue("test", logger));

        var removeResult = valueTable.Remove("test");

        Assert.True(removeResult);
        Assert.Throws<NullReferenceException>(() => valueTable.Get("test"));
    }
}
