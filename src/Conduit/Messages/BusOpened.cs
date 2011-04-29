using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    [ConduitMessageAttribute(Uri, true)]
    public class BusOpened : Message
    {
        public const string Uri = "http://Conduit/Messages/BusOpened";
    }
}
