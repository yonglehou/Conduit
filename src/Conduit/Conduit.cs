using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit
{
    public class Conduit : 
        IHandle<FindAvailableServices>,
        IHandle<AnnounceComponentIdentity>
    {
        private IServiceBus serviceBus = null;
        private List<string> capabilities = null;

        public Conduit(IServiceBus serviceBus)
            : this(serviceBus, null)
        {
        }

        public Conduit(IServiceBus serviceBus, List<ConduitComponent> components)
        {
            this.Id = Guid.NewGuid();
            this.serviceBus = serviceBus;
            this.capabilities = new List<string>();

            this.Bus = new MessageBus(serviceBus);
            this.Bus.Subscribe(this);

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

        public Guid Id { get; private set; }

        private string name = string.Empty;
        ///// <summary>
        ///// This is the name of your Component.
        ///// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = this.GetType().Name;
                }
                return name;
            }
        }

        private string ns = string.Empty;
        ///// <summary>
        ///// Namespace identifier for the Component within the distributed system. Using a Uri is suggested.
        ///// </summary>
        ///// <example>
        ///// http://company.com/component/mycomponent
        ///// </example>
        public string Namespace
        {
            get
            {
                if (string.IsNullOrEmpty(ns))
                {
                    Type attributeType = typeof(ConduitAttribute);
                    object[] attributes = this.GetType().GetCustomAttributes(attributeType, true);
                    if (attributes.Count() > 0)
                    {
                        ConduitAttribute attrib = attributes[0] as ConduitAttribute;
                        if (ns != null)
                        {
                            ns = attrib.Namespace;
                        }
                    }
                }
                return ns;
            }
        }

        /// <summary>
        /// List of Components available in the Conduit service.
        /// </summary>
        public OptimizedObservableCollection<ConduitComponent> Components { get; private set; }

        public IMessageBus Bus { get; private set; }

        void Components_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Handle when a new component is added to the conduit.
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    ConduitComponent component = item as ConduitComponent;
                    if (component != null)
                    {
                        component.Events = this.Bus;
                        Bus.Subscribe(component);
                    }
                }
            }
        }

        public void Open()
        {
            serviceBus.Open();

            Bus.Publish<FindAvailableComponents>();
            Bus.Publish<BusOpened>();
        }

        #region Message Handling
        public void Handle(FindAvailableServices message)
        {
            List<string> mergedCapabilities = new List<string>();

            // Add the capabilities from this Conduit.
            mergedCapabilities.AddRange(MessageHelper.GetCapabilities(this));

            // Add the capabilities from all the Components in this Conduit.
            mergedCapabilities.AddRange(capabilities);

            Bus.Publish<AnnounceServiceIdentity>(new AnnounceServiceIdentity(
                this.Name,
                this.Namespace,
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
