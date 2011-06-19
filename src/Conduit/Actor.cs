using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit
{
    public abstract class Actor :        
        IHandle<FindAvailableActors>
    {
        private static Type type = null;
        private ConduitNode node = null;

        public Actor()
            : this(Guid.NewGuid())
        {
        }

        public Actor(ConduitNode node)
            : this(Guid.NewGuid(), node)
        {
        }

        public Actor(Guid id)
            : this(id, Actor.ResolveNode())
        {
        }

        private Actor(Guid id, ConduitNode node)
        {
            this.Id = id;
            this.node = node;

            this.MessageBus.Subscribe(this);

            if (node.Opened)
            {
                this.PublishSelf<BusOpened>(new BusOpened());
            }
        }

        private static ConduitNode ResolveNode()
        {
            try
            {
                ConduitNode node = ServiceLocator.Current.ResolveAll<ConduitNode>().Single();
                return node;
            }
            catch (Exception)
            {
                throw new MultipleNodesException();
            }
        }

        public Guid Id { get; private set; }
        public Guid NodeId { get; internal set; }

        private IMessageBus messageBus = null;
        private IMessageBus MessageBus
        {
            get
            {
                if (messageBus != null)
                {
                    return messageBus;
                }
                else
                {
                    if (ServiceLocator.Current.CanResolve<IMessageBus>(node.Id.ToString()))
                    {
                        messageBus = ServiceLocator.Current.Resolve<IMessageBus>(node.Id.ToString());
                        return messageBus;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        private ILog log = null;
        private ILog Log
        {
            get
            {
                if (log != null)
                {
                    return log;
                }
                else
                {
                    if (ServiceLocator.Current.CanResolve<ILog>())
                    {
                        log = ServiceLocator.Current.Resolve<ILog>();
                        return log;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void Subscribe(object instance)
        {
            MessageBus.Subscribe(instance);
        }

        public void Unsubscribe(object instance)
        {
            MessageBus.Unsubscribe(instance);
        }

        public void Publish<T>() where T : Message, new()
        {
            MessageBus.Publish<T>();
        }

        public void Publish<T>(bool local) where T : Message, new()
        {
            MessageBus.Publish<T>(local);
        }

        public void Publish<T>(T message) where T : Message
        {
            MessageBus.Publish<T>(message);
        }

        public void Publish<T>(T message, bool local) where T : Message
        {
            MessageBus.Publish<T>(message, local);
        }

        private void PublishSelf<T>(T message)
        {
            ((MessageBus)MessageBus).SelfPublish(message, this);
        }

        #region Message Handling
        public void Handle(FindAvailableActors message)
        {
            List<string> capabilities = MessageHelper.GetCapabilities(this);

            if (type == null)
            {
                type = this.GetType();
            }

            MessageBus.Publish<AnnounceActorIdentity>(new AnnounceActorIdentity(
                type.Name,
                type.FullName,
                capabilities
                ), true);
        }
        #endregion
    }
}
