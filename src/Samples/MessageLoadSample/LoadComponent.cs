using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;
using MessageLoadSample.Messages;
using MessageLoadSample.Messages.Commands;
using MessageLoadSample.Messages.Queries;
using System.Threading;

namespace MessageLoadSample
{
    [ConduitComponent("http://Conduit/Samples/MessageLoadSample.Messages/LoadComponent")]
    public class LoadComponent : ConduitComponent,
        IHandle<TestMessage>,
        IHandle<QueryStatus>,
        IHandle<StatusMessage>,
        IHandle<StartCommand>,
        IHandle<ClearCommand>
    {
        private static readonly object lockReceived = new object();
        private int received = 0;
        private int count = 0;
        private DateTimeOffset startTime = DateTimeOffset.MinValue;
        private DateTimeOffset endTime = DateTimeOffset.MinValue;
        private TimeSpan duration = TimeSpan.Zero;
        private double rate = 0.0d;

        public void Handle(TestMessage message)
        {
            lock (lockReceived)
            {
                received++;
                if (received == count)
                {
                    endTime = DateTimeOffset.Now;
                    duration = endTime - startTime;
                    rate = received / duration.TotalSeconds;
                }
            }
        }

        public void Handle(QueryStatus message)
        {
            this.Events.Publish<StatusMessage>(new StatusMessage(this.Id, received, duration, rate));
        }

        public void Handle(StartCommand message)
        {
            received = 0;
            startTime = DateTimeOffset.Now;
            count = message.Count;

            for (int i = 0; i < count; i++)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Events.Publish<TestMessage>(new TestMessage(i));
                });
            }
        }

        public void Handle(StatusMessage message)
        {
            Console.WriteLine();

            Console.WriteLine(string.Format("{0}\n\tReceived:\t{1}\n\tDuration:\t{2}\n\tRate:\t\t{3:0.00} msgs/second",
                message.Id,
                message.Received,
                message.Duration.ToString(),
                message.RatePerSecond));

            Console.Write("Command>");
        }

        public void Handle(ClearCommand message)
        {
            Console.Clear();
            Console.Write("Command>");
        }
    }
}
