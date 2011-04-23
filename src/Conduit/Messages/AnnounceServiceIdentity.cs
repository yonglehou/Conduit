using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    [ConduitMessageAttribute(Uri)]
    public class AnnounceServiceIdentity : AnnounceIdentity
    {
        public const string Uri = "http://Conduit/Messages/AnnounceServiceIdentity";

        public AnnounceServiceIdentity(string name, string @namespace, IList<string> capabilities)
            : base(name, @namespace, capabilities)
        {

        }
    }
}
