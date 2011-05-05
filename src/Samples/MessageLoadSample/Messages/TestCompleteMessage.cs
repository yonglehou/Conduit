using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageLoadSample.Messages
{
    public class TestCompleteMessage : StatusMessage
    {
        public TestCompleteMessage()
        {

        }

        public TestCompleteMessage(Guid id, int received, TimeSpan duration, double ratePerSecond)
            : base(id, received, duration, ratePerSecond)
        {

        }
    }
}
