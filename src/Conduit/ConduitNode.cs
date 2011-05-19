using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit
{
    public class ConduitNode : 
        IMessageBus,
        IHandle<FindAvailableServices>,
        IHandle<AnnounceComponentIdentity>
    {
        private static Type type = null;

        private IServiceBus serviceBus = null;
        private List<string> capabilities = null;
        private bool opened = false;

        public ConduitNode(IServiceBus serviceBus)
            : this(serviceBus, null, null)
        {
        }

        public ConduitNode(IServiceBus serviceBus, List<ConduitComponent> components)
        {

        }

        public ConduitNode(IServiceBus serviceBus, List<ConduitComponent> components, ILog log)
        {
            if (log == null)
            {
                // Setup the default logger.
                log = new Log();
            }
            this.Log = log;

            this.Id = Guid.NewGuid();            
            Log.Info("Starting Conduit: " + this.Id);

            this.serviceBus = serviceBus;
            this.capabilities = new List<string>();

            this.Bus = new MessageBus(serviceBus, this.Log);

            if (this.Components == null)
            {
                this.Components = new OptimizedObservableCollection<ConduitComponent>();
                this.Components.CollectionChanged += 
                    new NotifyCollectionChangedEventHandler(Components_CollectionChanged);
            }

            if (components != null)
            {
                this.Components.AddRange(components);
            }
        }

        public ILog Log { get; private set; }

        public Guid Id { get; private set; }

        /// <summary>
        /// List of Components available in the Conduit service.
        /// </summary>
        public OptimizedObservableCollection<ConduitComponent> Components { get; private set; }

        private IMessageBus Bus { get; set; }

        void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (opened)
            {
                // Handle when a new component is added to the conduit.
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        ConduitComponent component = item as ConduitComponent;
                        if (component != null)
                        {
                            component.NodeId = this.Id;
                            component.Log = Log;
                            component.Bus = this;
                            Bus.Subscribe(component);
                        }
                    }
                }
            }
        }

        public void Open()
        {
            if (serviceBus != null)
            {
                serviceBus.Open();
            }

            // Initialize subscriptions
            Bus.Subscribe(this);

            foreach (ConduitComponent component in this.Components)
            {
                component.NodeId = this.Id;
                component.Bus = this;
                component.Log = this.Log;
                Bus.Subscribe(component);
            }

            Publish<FindAvailableComponents>(true);
            Publish<BusOpened>(true);

            Publish<FindAvailableServices>();

            opened = true;
        }

        public void Subscribe(object instance)
        {
            Bus.Subscribe(instance);
        }

        public void Unsubscribe(object instance)
        {
            Bus.Unsubscribe(instance);
        }

        public void Publish<T>() where T : Message, new()
        {
            T message = ObjectActivator.New<T>();
            message.SourceId = this.Id;
            Bus.Publish<T>(message);
        }

        public void Publish<T>(bool local) where T : Message, new()
        {
            T message = ObjectActivator.New<T>();

            message.SourceId = this.Id;
            Bus.Publish<T>(message, local);
        }

        public void Publish<T>(T message) where T : Message
        {
            message.SourceId = this.Id;
            Bus.Publish<T>(message);
        }

        public void Publish<T>(T message, bool local) where T : Message
        {
            message.SourceId = this.Id;
            Bus.Publish<T>(message, local);
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

            Publish<AnnounceServiceIdentity>(new AnnounceServiceIdentity(
                type.Name,
                type.FullName,
                mergedCapabilities
                ));
        }

        public void Handle(AnnounceComponentIdentity message)
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
