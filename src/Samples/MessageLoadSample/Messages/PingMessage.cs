using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages
{
    public class PingMessage : Message
    {
        public PingMessage()
        {
        }

        public PingMessage(Guid requestedId)
        {
            this.RequestedId = requestedId;
        }

        public Guid RequestedId { get; set; }
    }
}
