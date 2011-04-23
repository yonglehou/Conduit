using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit.Messages;

namespace Conduit.Messages.Queries
{
    [ConduitMessageAttribute(Uri, true)]
    public class FindAvailableComponents : Message
    {
        public const string Uri = "http://Conduit/Messages/Queries/FindAvailableComponents";
    }
}
