using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexer.Tokens;

namespace Parser.Node.Interfaces
{
    public interface IValueNode : INode
    {
        Token Value { get; set; }
    }
}