using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewPirateLexer.Tokens;

namespace NewParserTest.Node.Interfaces
{
    public interface IValueNode
    {
        Token Value { get; set; }
    }
}