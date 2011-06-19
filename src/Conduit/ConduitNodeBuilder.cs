using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public sealed class ConduitNodeBuilder
    {
        internal IServiceBus ServiceBus { get; private set; }
        internal ILog Log { get; private set; }
        internal bool AutoOpen { get; private set; }
        internal TimeSpan DiscoveryFrequency { get; private set; }

        public static implicit operator ConduitNode(ConduitNodeBuilder builder)
        {
            ConduitNode node = new ConduitNode(builder.ServiceBus, builder.Log, builder.DiscoveryFrequency);
            if (builder.AutoOpen)
            {
                node.Open();
            }
            return node;
        }

        public ConduitNodeBuilder WithServiceBus(IServiceBus serviceBus)
        {
            this.ServiceBus = serviceBus;
            return this;
        }

        public ConduitNodeBuilder WithLog(ILog log)
        {
            this.Log = log;
            return this;
        }

        public ConduitNodeBuilder WithDiscoveryFrequency(TimeSpan frequency)
        {
            this.DiscoveryFrequency = frequency;
            return this;
        }

        public ConduitNodeBuilder Open()
        {
            this.AutoOpen = true;
            return this;
        }
    }
}
