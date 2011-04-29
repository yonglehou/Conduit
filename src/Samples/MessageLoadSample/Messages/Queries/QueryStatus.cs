using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages.Queries
{
    [ConduitMessage("http://Conduit/Samples/MessageLoadSample/QueryStatus")]
    public class QueryStatus : Message
    {
        public QueryStatus()
        {

        }
    }
}
