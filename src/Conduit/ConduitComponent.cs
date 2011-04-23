using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit
{
    public abstract class ConduitComponent :
        IHandle<FindAvailableComponents>
    {
        public ConduitComponent()
        {
            this.Id = Guid.NewGuid();
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
                    Type attributeType = typeof(ConduitComponentAttribute);
                    object[] attributes = this.GetType().GetCustomAttributes(attributeType, true);
                    if (attributes.Count() > 0)
                    {
                        ConduitComponentAttribute attrib = attributes[0] as ConduitComponentAttribute;
                        if (ns != null)
                        {
                            ns = attrib.Namespace;
                        }
                    }
                }
                return ns;
            }
        }

        protected internal IMessageBus Events { get; internal set; }

        #region Message Handling
        public void Handle(FindAvailableComponents message)
        {
            List<string> capabilities = MessageHelper.GetCapabilities(this);

            Events.Publish<AnnounceComponentIdentity>(new AnnounceComponentIdentity(
                this.Name,
                this.Namespace,
                capabilities
                ));
        }
        #endregion
    }
}
