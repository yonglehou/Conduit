using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages
{
    public class ConsistencyMessage : Message
    {
        public ConsistencyMessage()
        {

        }

        public ConsistencyMessage(Dictionary<int, int> records)
        {
            this.Records = records;
        }

        public Dictionary<int, int> Records { get; set; }
    }
}
