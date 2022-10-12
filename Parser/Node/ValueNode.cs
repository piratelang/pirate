using System.Security.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser.Node.Interfaces;
using Lexer.Tokens;

namespace Parser.Node
{
    public class ValueNode : INode, IValueNode
    {
        public Token Value { get; set; }

        public ValueNode(Token value)
        {
            Value = value;
        }

        public string Display()
        {
            return $"({Value.Display()})";
        }
    }
}