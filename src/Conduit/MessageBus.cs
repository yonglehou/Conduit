namespace Conduit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Messages;

    /// <summary>
    ///   Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "instance">The instance to subscribe for event publication.</param>
        void Subscribe(object instance);

        /// <summary>
        ///   Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "instance">The instance to unsubscribe.</param>
        void Unsubscribe(object instance);

        void Publish<T>() where T : Message, new();

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        void Publish<T>(T message) where T : Message;
    }

    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class MessageBus : IMessageBus
    {
        //static readonly ILog Log = LogManager.GetLog(typeof(EventAggregator));
        readonly List<Handler> handlers = new List<Handler>();

        readonly IServiceBus bus = null;

        readonly Dictionary<Type, ConduitMessageAttribute> messageAttributes = 
            new Dictionary<Type, ConduitMessageAttribute>();

        public MessageBus()
        {
        }

        public MessageBus(IServiceBus bus)
        {
            this.bus = bus;
        }

        /// <summary>
        ///   Subscribes an instance to all events declared through implementations of <see cref = "IHandle{T}" />
        /// </summary>
        /// <param name = "instance">The instance to subscribe for event publication.</param>
        public void Subscribe(object instance)
        {
            lock (handlers)
            {
                if (handlers.Any(x => x.Matches(instance)))
                    return;

                //Log.Info("Subscribing {0}.", instance);
                handlers.Add(new Handler(instance));
            }
        }

        /// <summary>
        ///  Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "instance">The instance to unsubscribe.</param>
        public void Unsubscribe(object instance)
        {
            lock (handlers)
            {
                var found = handlers.FirstOrDefault(x => x.Matches(instance));

                if (found != null)
                    handlers.Remove(found);
            }
        }

        public void Publish<T>() where T : Message, new() 
        {
            T message = new T();
            if (message != null)
            {
                Publish<T>(message);
            }
            else
            {
                throw new NullReferenceException("Unable to publish message of type: " + typeof(T).Name);
            }
        }

        public void Publish<T>(T message) where T : Message
        {
            // Publish to the local EventAggregator loop first.
            this.Publish((object)message);

            // Then send over the Service Bus wire.
            if (bus != null)
            {
                // If the message is local only, don't send it over the Service Bus wire.
                ConduitMessageAttribute messageAttribute = AddMessageInfo(message);
                if (messageAttribute != null && messageAttribute.Local == false)
                {
                    bus.Publish<T>(message);
                }
            }
        }

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        private void Publish(object message)
        {
            Handler[] toNotify;
            lock (handlers)
                toNotify = handlers.ToArray();

            //Execute.OnUIThread(() =>
            //{
                //Log.Info("Publishing {0}.", message);
                var messageType = message.GetType();
                var dead = toNotify.Where(handler => !handler.Handle(messageType, message));

                if (dead.Any())
                {
                    lock (handlers)
                        dead.Apply(x => handlers.Remove(x));
                }
            //});
        }

        private ConduitMessageAttribute AddMessageInfo(object message)
        {
            ConduitMessageAttribute messageAttribute;
            Type messageType = message.GetType();
            if (!messageAttributes.TryGetValue(messageType, out messageAttribute))
            {
                messageAttribute = MessageHelper.GetMessageInfo(messageType);
                if (messageAttribute != null)
                {
                    // We hold a cache of message type and message attribute
                    // so that we can pull this info out and make decisions.
                    messageAttributes.Add(messageType, messageAttribute);
                }
                if (messageAttribute == null || string.IsNullOrWhiteSpace(messageAttribute.Namespace))
                {
                    throw new MessageNamespaceMissingException("Message does not define a namespace");
                }
            }
            return messageAttribute;
        }

        class Handler
        {
            readonly WeakReference reference;
            readonly Dictionary<Type, MethodInfo> supportedHandlers = new Dictionary<Type, MethodInfo>();

            public Handler(object handler)
            {
                reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("Handle");
                    supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance)
            {
                return reference.Target == instance;
            }

            public bool Handle(Type messageType, object message)
            {
                var target = reference.Target;
                if (target == null)
                    return false;

                foreach (var pair in supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        pair.Value.Invoke(target, new[] { message });
                        return true;
                    }
                }
                return true;
            }
        }
    }
}