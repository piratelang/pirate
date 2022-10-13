using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser.Node.Interfaces;
using Common;

namespace Parser
{
    [Serializable]
    public class Scope
    {
        public List<INode> Nodes { get; private set; }
        private Logger Logger { get; set; }

        public Scope(Logger logger)
        {
            Logger = logger;
            Nodes = new();
        }

        public bool AddNode(INode node)
        {
            try
            {
                Nodes.Add(node);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Scope.AddNode() returned an error of {ex.ToString()}", this.GetType().Name, Common.Enum.LogType.ERROR);
                Console.WriteLine(ex);
                return false;
            }
            
        }
    }
}