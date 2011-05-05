using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages.Queries
{
    public class QueryPing : Message
    {
        public QueryPing()
        {
        }

        public QueryPing(Guid requestedId)
        {
            this.RequestedId = requestedId;
        }

        public Guid RequestedId { get; set; }
    }
}
