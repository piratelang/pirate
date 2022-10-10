using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NewParserTest.Node.Interfaces;

namespace NewParserTest
{
    public class Scope
    {
        public List<INode> Nodes { get; set; }

        public bool AddNode(INode node)
        {
            try
            {
                Nodes.Append(node);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
            
        }
    }
}