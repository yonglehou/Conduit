using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit
{
    public abstract class ConduitComponent :
        IMessageBus,
        IHandle<FindAvailableComponents>
    {
        private static Type type = null;

        public ConduitComponent()
            : this(Guid.NewGuid())
        {
        }

        public ConduitComponent(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; private set; }
        public Guid NodeId { get; internal set; }

        internal IMessageBus Bus { get; set; }
        protected internal ILog Log { get; internal set; }

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
            Bus.Publish<T>();
        }

        public void Publish<T>(bool local) where T : Message, new()
        {
            Bus.Publish<T>(local);
        }

        public void Publish<T>(T message) where T : Message
        {
            Bus.Publish<T>(message);
        }

        public void Publish<T>(T message, bool local) where T : Message
        {
            Bus.Publish<T>(message, local);
        }

        #region Message Handling
        public void Handle(FindAvailableComponents message)
        {
            List<string> capabilities = MessageHelper.GetCapabilities(this);

            if (type == null)
            {
                type = this.GetType();
            }

            Bus.Publish<AnnounceComponentIdentity>(new AnnounceComponentIdentity(
                type.Name,
                type.FullName,
                capabilities
                ), true);
        }
        #endregion
    }
}
