﻿namespace Conduit
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
        void Publish<T>(bool local) where T : Message, new();

        /// <summary>
        ///   Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        void Publish<T>(T message) where T : Message;
        void Publish<T>(T message, bool local) where T : Message;

        IServiceBus ServiceBus { get; set; }
        //ILog Log { get; set; }
    }

    /// <summary>
    /// Enables loosely-coupled publication of and subscription to events.
    /// </summary>
    public class MessageBus : IMessageBus
    {
        readonly List<Handler> handlers = new List<Handler>();
        private ILog log = null;

        private IServiceBus serviceBus = null;
        public IServiceBus ServiceBus 
        {
            get { return serviceBus; }
            set
            {
                if (serviceBus != null)
                {
                    this.serviceBus.MessageReceived -= new MessageReceivedHandler(serviceBus_MessageReceived);
                }

                serviceBus = value;
                if (serviceBus != null)
                {
                    this.serviceBus.MessageReceived += new MessageReceivedHandler(serviceBus_MessageReceived);
                }

                SubscribeHandlers();
            }
        }

        public MessageBus(IServiceBus serviceBus, ILog log)
        {
            ServiceBus = serviceBus;
            this.log = log;
        }

        public void SubscribeHandlers()
        {
            lock (handlers)
            {
                foreach (Handler instance in handlers)
                {
                    SubscribeToServiceBus(instance);
                }
            }
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

                SubscribeToServiceBus(instance);
            }
        }

        private void SubscribeToServiceBus(object instance)
        {
            // Subscribe to the service bus.
            if (ServiceBus != null)
            {
                // Get a list of IHandle<T> this object supports.
                string handleName = typeof(IHandle).Name;
                Type[] interfaces = instance.GetType().GetInterfaces();

                foreach (Type interfaceType in interfaces)
                {
                    if (interfaceType.IsGenericType)
                    {
                        if (interfaceType.Name.StartsWith(handleName))
                        {
                            MethodInfo method = ServiceBus.GetType().GetMethod("Subscribe");
                            if (method.IsGenericMethod)
                            {
                                // Subscribe for the message type.
                                Type[] messageTypes = interfaceType.GetGenericArguments();
                                if (messageTypes.Length > 0)
                                {
                                    MethodInfo m = method.MakeGenericMethod(messageTypes[0]);
                                    m.Invoke(ServiceBus, null);
                                }
                            }
                        }
                    }
                }
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
            this.Publish<T>(false);
        }

        public void Publish<T>(bool local) where T : Message, new() 
        {
            T message = new T();
            if (message != null)
            {
                Publish<T>(message, local);
            }
            else
            {
                throw new NullReferenceException("Unable to publish message of type: " + typeof(T).Name);
            }
        }

        public void Publish<T>(T message) where T : Message
        {
            Publish<T>(message, false);
        }

        public void Publish<T>(T message, bool local) where T : Message
        {
            // If the service bus is null then we might as well treat this
            // as a local only message.
            if (ServiceBus == null)
            {
                local = true;
            }

            // If the message is local only, don't send it over the Service Bus wire.
            if (local)
            {
                // Send only to the local message bus.
                if (log.IsInfoEnabled)
                {
                    log.Info(string.Format("MSG-SEND: {0}",
                        message.GetType().Name));
                }

                this.Publish((object)message);
            }
            else
            {
                if (ServiceBus != null)
                {
                    // Send to the service bus.
                    if (log.IsInfoEnabled)
                    {
                        log.Info(string.Format("BUS-SEND: {0}",
                            message.GetType().Name));
                    }

                    ServiceBus.Publish<T>(message);
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

        internal void SelfPublish(object message, object target)
        {
            Handler[] toNotify;
            lock (handlers)
                toNotify = handlers.ToArray();

            var messageType = message.GetType();
            var toNotify2 = toNotify.Where(handler => handler.Matches(target));
            var dead = toNotify2.Where(handler => !handler.Handle(messageType, message));

            if (dead.Any())
            {
                lock (handlers)
                    dead.Apply(x => handlers.Remove(x));
            }
        }

        private void serviceBus_MessageReceived(Message message)
        {
            if (log.IsInfoEnabled)
            {
                log.Info(string.Format("BUS-RECV: {0}",
                    message.GetType().Name));
            }

            // Message received from the service bus. Publish it on the local message bus.
            Publish((object)message);
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