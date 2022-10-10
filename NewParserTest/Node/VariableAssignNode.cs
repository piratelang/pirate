using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewParserTest.Node.Interfaces;

namespace NewParserTest.Node;

public class VariableAssignNode : INode
{
    public INode Identifier { get; set; }
    public INode Value { get; set; }

    public VariableAssignNode(INode identifier, INode value)
    {
        Identifier = identifier;
        Value = value;
    }

    public string Display()
    {
        return $"({Identifier.Display()} = {Value.Display()})";
    }
}