using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Conduit.Tests
{
    class FakeServiceBus : IServiceBus
    {
        public event MessageReceivedHandler MessageReceived;

        public void Open()
        {
        }

        public void Publish<T>(T message) where T : Message
        {
            // Mimic the behavior of a real service bus where the msgs bounce back.
            // TODO: We could remove this dependency if the message bus looped the message.
            if (MessageReceived != null)
            {
                ThreadPool.QueueUserWorkItem((object state) =>
                {
                    MessageReceived(message);
                });
            }
        }

        public void Subscribe<T>() where T : Message
        {
        }

        public void Unsubscribe<T>() where T : Message
        {
        }

        public void Dispose()
        {
        }
    }
}
