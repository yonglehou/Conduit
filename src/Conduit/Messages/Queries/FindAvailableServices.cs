using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages.Queries
{
    [ConduitMessageAttribute(Uri)]
    public class FindAvailableServices : Message
    {
        public const string Uri = "http://Conduit/Messages/Queries/FindAvailableServices";
    }
}
