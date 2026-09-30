using Pirate.Syntax;
using Pirate.Syntax.Nodes;

namespace Pirate.Semantics.Test;

public class SymbolTableTests
{
    private static VariableSymbol Variable(string name, int index, bool isConst = false) =>
        new(name, index, SymbolScope.Local, PirateType.Int, isConst);

    [Fact]
    public void Define_Success_ReturnsNull()
    {
        var table = new SymbolTable();
        Assert.Null(table.Define(Variable("x", 0)));
    }

    [Fact]
    public void Define_DuplicateName_ReturnsExistingSymbol()
    {
        var table = new SymbolTable();
        var first = Variable("x", 0);
        table.Define(first);

        var result = table.Define(Variable("x", 1));

        Assert.Same(first, result);
    }

    [Fact]
    public void Resolve_FindsOwnSymbol()
    {
        var table = new SymbolTable();
        table.Define(Variable("x", 0));

        Assert.NotNull(table.Resolve("x"));
    }

    [Fact]
    public void Resolve_UnknownName_ReturnsNull()
    {
        var table = new SymbolTable();
        Assert.Null(table.Resolve("nope"));
    }

    [Fact]
    public void Resolve_FallsThroughToParent()
    {
        var global = new SymbolTable();
        global.Define(new FunctionSymbol("main", 0, SymbolScope.Global, [], PirateType.Void));
        var local = new SymbolTable(global);

        var resolved = local.Resolve("main");

        Assert.IsType<FunctionSymbol>(resolved);
    }

    [Fact]
    public void Resolve_LocalShadowsParent()
    {
        var global = new SymbolTable();
        var globalFunction = new FunctionSymbol("f", 0, SymbolScope.Global, [], PirateType.Void);
        global.Define(globalFunction);
        var local = new SymbolTable(global);
        var localVariable = Variable("f", 0);
        local.Define(localVariable);

        Assert.Same(localVariable, local.Resolve("f"));
        Assert.Same(globalFunction, global.Resolve("f"));
    }

    [Fact]
    public void Define_DoesNotSeeSiblingScope()
    {
        var global = new SymbolTable();
        var scopeA = new SymbolTable(global);
        var scopeB = new SymbolTable(global);
        scopeA.Define(Variable("x", 0));

        Assert.Null(scopeB.Resolve("x"));
    }

    [Fact]
    public void ScopeLevel_IncrementsPerParent()
    {
        var global = new SymbolTable();
        var local = new SymbolTable(global);
        var nested = new SymbolTable(local);

        Assert.Equal(0, global.ScopeLevel);
        Assert.Equal(1, local.ScopeLevel);
        Assert.Equal(2, nested.ScopeLevel);
    }

    [Fact]
    public void Count_TracksDefinitionsInThisScopeOnly()
    {
        var global = new SymbolTable();
        global.Define(new FunctionSymbol("a", 0, SymbolScope.Global, [], PirateType.Void));
        global.Define(new FunctionSymbol("b", 1, SymbolScope.Global, [], PirateType.Void));
        var local = new SymbolTable(global);
        local.Define(Variable("x", 0));

        Assert.Equal(2, global.Count);
        Assert.Equal(1, local.Count);
    }
}

public class BuiltinRegistryTests
{
    [Theory]
    [InlineData("Standard.Terminal.Print")]
    [InlineData("Standard.Terminal.PrintLine")]
    [InlineData("Standard.Terminal.Read")]
    [InlineData("Standard.String.Length")]
    [InlineData("Standard.String.CharAt")]
    [InlineData("Standard.String.Concat")]
    [InlineData("Standard.String.Split")]
    [InlineData("Standard.String.IndexOf")]
    [InlineData("Standard.String.LastIndexOf")]
    [InlineData("Standard.String.CharCodeAt")]
    public void Lookup_KnownBuiltin_ReturnsEntry(string name) =>
        Assert.NotNull(BuiltinRegistry.Lookup(name));

    [Theory]
    [InlineData("Standard.Terminal.Prnt")]
    [InlineData("Standard.Math.Abs")]
    [InlineData("Print")]
    [InlineData("")]
    public void Lookup_UnknownName_ReturnsNull(string name) =>
        Assert.Null(BuiltinRegistry.Lookup(name));

    [Fact]
    public void Signatures_MatchStandardLibrarySurface()
    {
        var print = BuiltinRegistry.Lookup("Standard.Terminal.Print")!;
        Assert.Equal([PirateType.String], print.Parameters);
        Assert.Equal(PirateType.Void, print.ReturnType);

        var read = BuiltinRegistry.Lookup("Standard.Terminal.Read")!;
        Assert.Empty(read.Parameters);
        Assert.Equal(PirateType.String, read.ReturnType);

        var charAt = BuiltinRegistry.Lookup("Standard.String.CharAt")!;
        Assert.Equal([PirateType.String, PirateType.Int], charAt.Parameters);
        Assert.Equal(PirateType.Char, charAt.ReturnType);

        var split = BuiltinRegistry.Lookup("Standard.String.Split")!;
        Assert.Equal(new[] { PirateType.String, PirateType.String }, split.Parameters);
        Assert.Equal(PirateType.ArrayOf(ScalarType.String), split.ReturnType);
    }

    [Fact]
    public void Indexes_AreStableAndUnique()
    {
        var indexes = BuiltinRegistry.All.Select(e => e.Index).ToList();
        Assert.Equal(indexes.Count, indexes.Distinct().Count());
        Assert.Equal(Enumerable.Range(0, BuiltinRegistry.All.Count), indexes);
    }
}
