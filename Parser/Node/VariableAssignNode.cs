using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser.Node.Interfaces;
using Lexer.Tokens;

namespace Parser.Node;

public class VariableAssignNode : INode
{
    private Token TypeToken { get; set; }
    private INode Identifier { get; set; }
    private INode Value { get; set; }

    public VariableAssignNode(Token typeToken, INode identifier, INode value)
    {
        TypeToken= typeToken;
        Identifier = identifier;
        Value = value;
    }

    public override string ToString()
    {
        return $"({Identifier.ToString()} = {Value.ToString()})";
    }
}