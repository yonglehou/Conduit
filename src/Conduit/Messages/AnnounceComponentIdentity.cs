using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    [ConduitMessageAttribute(Uri, true)]
    public class AnnounceComponentIdentity : AnnounceIdentity
    {
        public const string Uri = "http://Conduit/Messages/AnnounceComponentIdentity";

        public AnnounceComponentIdentity(string name, string @namespace, IList<string> capabilities)
            : base(name, @namespace, capabilities)
        {

        }
    }
}
