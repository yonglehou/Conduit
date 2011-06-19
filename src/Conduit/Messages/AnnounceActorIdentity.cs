using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public class AnnounceActorIdentity : AnnounceIdentity
    {
        public AnnounceActorIdentity()
        {

        }

        public AnnounceActorIdentity(string name, string type, IList<string> capabilities)
            : base(name, type, capabilities)
        {
        }
    }
}
