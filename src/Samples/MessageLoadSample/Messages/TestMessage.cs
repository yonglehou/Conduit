using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages
{
    public class TestMessage : Message
    {
        public TestMessage()
        {

        }

        public TestMessage(int count)
        {
            this.Count = count;
        }

        public int Count { get; set; }
    }
}
