using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;
using System.Threading;

namespace Conduit
{
    public class ConduitNode : 
        IHandle<FindAvailableServices>,
        IHandle<AnnounceActorIdentity>
    {
        // Constants
        private const int DefaultDiscoveryFrequencyMilliseconds = 1000;

        private static Type type = null;

        private Timer discoveryTimer;
        private List<string> capabilities = null;

        public ConduitNode()
            : this(null, null, TimeSpan.FromMilliseconds(DefaultDiscoveryFrequencyMilliseconds))
        {
        }

        public ConduitNode(IServiceBus serviceBus)
            : this(serviceBus, null, TimeSpan.FromMilliseconds(DefaultDiscoveryFrequencyMilliseconds))
        {
        }

        public ConduitNode(TimeSpan discoveryFrequency)
            : this(null, null, discoveryFrequency)
        {
        }

        public ConduitNode(IServiceBus serviceBus, ILog log, TimeSpan discoveryFrequency)
        {
            this.Id = Guid.NewGuid();
            ServiceLocator.Current.RegisterInstance<ConduitNode>(this.Id.ToString(), this);

            if (log != null)
            {
                this.Log = log;
            }
            else
            {
                // Check to make sure the IoC container doesn't
                // have one already registered.
                if (this.Log == null)
                {
                    // Setup the default logger if the builder wasn't passed one.
                    this.Log = new Log();
                }
            }

            this.MessageBus = new MessageBus(serviceBus, this.Log);

            if (discoveryFrequency != TimeSpan.Zero)
            {
                this.DiscoveryFrequency = discoveryFrequency;
            }

            this.capabilities = new List<string>();
        }

        public static ConduitNodeBuilder Create()
        {
            return new ConduitNodeBuilder();
        }

        public Guid Id { get; private set; }

        private ILog log = null;
        public ILog Log
        {
            get
            {

                if (this.log != null)
                {
                    return this.log;
                }
                else if (ServiceLocator.Current.CanResolve<ILog>())
                {
                    this.log = ServiceLocator.Current.Resolve<ILog>();
                    return this.log;
                }
                else
                {
                    return null;
                }
            }

            internal set
            {
                this.log = value;
                if (value != null)
                {
                    ServiceLocator.Current.RegisterInstance<ILog>(value);
                }
            }
        }

        private IMessageBus messageBus = null;
        public IMessageBus MessageBus
        {
            get
            {

                if (this.messageBus != null)
                {
                    return this.messageBus;
                }
                else if (ServiceLocator.Current.CanResolve<IMessageBus>(this.Id.ToString()))
                {
                    this.messageBus = ServiceLocator.Current.Resolve<IMessageBus>(this.Id.ToString());
                    return this.messageBus;
                }
                else
                {
                    return null;
                }
            }

            internal set
            {
                this.messageBus = value;
                ServiceLocator.Current.RegisterInstance<IMessageBus>(this.Id.ToString(), value);
            }
        }

        private bool opened = false;
        public bool Opened
        {
            get { return opened; }
            private set { opened = value; }
        }

        private TimeSpan discoveryFrequency = TimeSpan.FromMilliseconds(DefaultDiscoveryFrequencyMilliseconds);
        public TimeSpan DiscoveryFrequency
        {
            get { return discoveryFrequency; }
            set
            {
                discoveryFrequency = value;

                if (discoveryTimer != null)
                {
                    discoveryTimer.Change((long) discoveryFrequency.TotalMilliseconds,
                                          (long) discoveryFrequency.TotalMilliseconds);
                }
            }
        }

        public void Open()
        {
            // Open the ServiceBus.
            if (this.MessageBus.ServiceBus != null)
            {
                this.MessageBus.ServiceBus.Open();
            }

            // Initialize subscriptions
            MessageBus.Subscribe(this);

            // Discover what actors exist in the process already in memory.
            MessageBus.Publish<FindAvailableActors>(true);
            MessageBus.Publish<BusOpened>(true);

            MessageBus.Publish<FindAvailableServices>();

            opened = true;

            discoveryTimer = new Timer(DiscoveryTimerCallback, null, this.DiscoveryFrequency, this.DiscoveryFrequency);
        }

        public void Close()
        {
            this.MessageBus.Unsubscribe(this);
            if (this.MessageBus.ServiceBus != null)
            {
                this.MessageBus.ServiceBus.Dispose();
            }

            ServiceLocator.Current.Remove<ILog>();
            ServiceLocator.Current.Remove<IMessageBus>(this.Id.ToString());
            ServiceLocator.Current.Remove<IServiceBus>(this.Id.ToString());
            ServiceLocator.Current.Remove<ConduitNode>(this.Id.ToString());

            opened = false;
        }

        private void DiscoveryTimerCallback(object state)
        {
            Console.WriteLine("TIMER");
            MessageBus.Publish<FindAvailableActors>(true);
            MessageBus.Publish<FindAvailableServices>();
        }

        #region Message Handling
        public void Handle(FindAvailableServices message)
        {
            if (type == null)
            {
                type = this.GetType();
            }

            List<string> mergedCapabilities = new List<string>();

            // Add the capabilities from this Conduit.
            mergedCapabilities.AddRange(MessageHelper.GetCapabilities(this));

            // Add the capabilities from all the Components in this Conduit.
            mergedCapabilities.AddRange(capabilities);

            MessageBus.Publish<AnnounceServiceIdentity>(new AnnounceServiceIdentity(
                type.Name,
                type.FullName,
                mergedCapabilities
                ));
        }

        public void Handle(AnnounceActorIdentity message)
        {
            foreach (string capability in message.Capabilities)
            {
                if (!capabilities.Contains(capability))
                {
                    capabilities.Add(capability);
                }
            }
        }
        #endregion
    }
}
