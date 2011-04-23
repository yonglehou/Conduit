using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public class ConduitMessageAttribute : Attribute
    {
        public ConduitMessageAttribute(string @namespace)
            : this(@namespace, false)
        {
        }

        public ConduitMessageAttribute(string @namespace, bool local)
        {
            this.Namespace = @namespace;
            this.Local = local;
        }

        public string Namespace { get; private set; }
        public bool Local { get; private set; }
    }
}
