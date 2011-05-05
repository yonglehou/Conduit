using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public delegate void MessageReceivedHandler(Message message);

    public interface IServiceBus : IDisposable
    {
        event MessageReceivedHandler MessageReceived;

        void Open();
        void Publish<T>(T message) where T : Message;
        void Subscribe<T>() where T : Message;
        void Unsubscribe<T>() where T : Message;
    }
}
