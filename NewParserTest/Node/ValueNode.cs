using System.Security.Principal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewParserTest.Node.Interfaces;
using NewPirateLexer.Tokens;

namespace NewParserTest.Node
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