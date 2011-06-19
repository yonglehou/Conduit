using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public class MultipleNodesException : Exception
    {
        public MultipleNodesException()
            : base("Multiple nodes have been created. Must associate this actor with a node.")
        {        
        }
    }
}
