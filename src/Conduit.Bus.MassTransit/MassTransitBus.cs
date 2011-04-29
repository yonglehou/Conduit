using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Castle.MicroKernel.Registration;
using MassTransit;
using MassTransit.Transports.Msmq;
using MassTransit.WindsorIntegration;
using MassTransit.Internal;

using IMassTransitServiceBus = MassTransit.IServiceBus;
using System.Threading;

namespace Conduit.Bus.MassTransit
{
    public class MassTransitBus : IServiceBus
    {
        public event MessageReceivedHandler MessageReceived;

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

        public void Subscribe<T>() where T : Message
        {
            container.Register(Component.For<Consumer<T>>().LifeStyle.Transient);            
            var consumer = container.Resolve<Consumer<T>>();

            if (consumer != null)
            {
                consumer.Bus = this;
                consumer.Start(bus);
            }
        }

        public void Publish<T>(T message) where T : Message
        {
            bus.Publish<T>(message);
        }

        internal void Inject(Message message)
        {
            ThreadPool.QueueUserWorkItem((o) =>
                {
                    MessageReceivedHandler evt = MessageReceived;
                    if (evt != null)
                    {
                        evt(message);
                    }
                });
        }
    }
}
