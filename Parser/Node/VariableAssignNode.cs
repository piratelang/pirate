using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser.Node.Interfaces;
using Lexer.Tokens;

namespace Parser.Node;

[Serializable]
public class VariableAssignNode : INode
{
    public Token TypeToken { get; set; }
    public IValueNode Identifier { get; set; }
    public INode Value { get; set; }

    public VariableAssignNode(Token typeToken, IValueNode identifier, INode value)
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