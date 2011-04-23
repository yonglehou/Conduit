using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public class ConduitAttribute : Attribute
    {
        public ConduitAttribute(string @namespace)
        {
            this.Namespace = @namespace;
        }

        public string Namespace { get; private set; }
    }
}
