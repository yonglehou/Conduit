using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public class AnnounceComponentIdentity : AnnounceIdentity
    {
        public AnnounceComponentIdentity(string name, string type, IList<string> capabilities)
            : base(name, type, capabilities)
        {
        }
    }
}
