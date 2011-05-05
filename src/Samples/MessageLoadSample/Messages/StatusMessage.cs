using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages
{
    public class StatusMessage : Message
    {
        public StatusMessage()
        {

        }

        public StatusMessage(Guid id, int received, TimeSpan duration, double ratePerSecond)
        {
            this.Id = id;
            this.Received = received;
            this.Duration = duration;
            this.RatePerSecond = ratePerSecond;
        }

        public Guid Id { get; set; }
        public int Received { get; set; }
        public TimeSpan Duration { get; set; }
        public double RatePerSecond { get; set; }
    }
}
