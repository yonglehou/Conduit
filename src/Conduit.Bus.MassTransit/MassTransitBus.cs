using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MassTransit;
using MassTransit.Transports.Msmq;
using MassTransit.WindsorIntegration;

using IMassTransitServiceBus = MassTransit.IServiceBus;

namespace Conduit.Bus.MassTransit
{
    public class MassTransitBus : IServiceBus
    {
        private bool disposed = false;
        private DefaultMassTransitContainer container = null;
        private IMassTransitServiceBus bus = null;

        public MassTransitBus()
        {
        }

        ~MassTransitBus()
        {
            Dispose(false);
        }

        // Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    container.Release(bus);
                    container.Dispose();
                }

                // If disposing is false, clean up unmanaged resources here.

                // Note disposing has been done.
                disposed = true;

            }
        }

        public void Open()
        {
            MsmqEndpointConfigurator.Defaults(config =>
            {
                config.CreateMissingQueues = true;
            });

            container = new DefaultMassTransitContainer("windsor.xml");
            bus = container.Resolve<IMassTransitServiceBus>();
        }

        public void Publish<T>(T message) where T : class
        {
            bus.Publish<T>(message);
        }

        public void Subscribe<T>() where T : class
        {
            bus.Subscribe<T>();
        }
    }
}
