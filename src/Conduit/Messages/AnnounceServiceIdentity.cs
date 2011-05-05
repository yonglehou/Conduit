using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public class AnnounceServiceIdentity : AnnounceIdentity
    {
        public AnnounceServiceIdentity(string name, string type, IList<string> capabilities)
            : base(name, type, capabilities)
        {

        }
    }
}
