using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MT = MassTransit;
using MassTransit;
using MassTransit.Internal;
using Conduit;

namespace Conduit.Bus.MassTransit
{
    public class Consumer<T> : Consumes<T>.All, IBusService where T : Message
    {
        private MT.IServiceBus transitBus;
        private UnsubscribeAction unsubscribeAction;

        internal MassTransitBus Bus { get; set; }

        public void Consume(T message)
        {
            Bus.Inject(message);
        }

        public void Dispose()
        {
            transitBus.Dispose();
        }

        public void Start(MT.IServiceBus transitBus)
        {
            this.transitBus = transitBus;
            unsubscribeAction = transitBus.Subscribe(this);
        }

        public void Stop()
        {
            unsubscribeAction();
        }
    }
}
