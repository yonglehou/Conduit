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
    public class LoadComponent : ConduitComponent,
        IHandle<TestMessage>,
        IHandle<QueryStatus>,
        IHandle<StatusMessage>,
        IHandle<StartCommand>,
        IHandle<ClearCommand>,
        IHandle<UnsubscribeCommand>,
        IHandle<QueryConsistency>,
        IHandle<QueryPing>,
        IHandle<PingMessage>
    {
        private static readonly object lockReceived = new object();
        private int received = 0;
        private int count = 0;
        private DateTimeOffset startTime = DateTimeOffset.MinValue;
        private DateTimeOffset endTime = DateTimeOffset.MinValue;
        private TimeSpan duration = TimeSpan.Zero;
        private double rate;
        private Dictionary<int, int> numbers = new Dictionary<int, int>();
        private DateTimeOffset startPingTime = DateTimeOffset.MinValue;

        public void Handle(TestMessage message)
        {
            lock (lockReceived)
            {
                received++;
                numbers.Add(received, message.Count);

                endTime = DateTimeOffset.Now;
                duration = endTime - startTime;
                rate = CalculateRate();
            }
        }

        public void Handle(QueryStatus message)
        {
            double rate = CalculateRate();
            this.Publish<StatusMessage>(new StatusMessage(this.Id, received, duration, rate));
        }

        public void Handle(StartCommand message)
        {
            numbers.Clear();

            received = 0;
            startTime = DateTimeOffset.Now;
            endTime = DateTimeOffset.MinValue;
            duration = TimeSpan.Zero;
            count = message.Count;

            for (int i = 0; i < count; i++)
            {
                if (message.Async)
                {
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        int current = int.Parse(o.ToString()) + 1;
                        Publish<TestMessage>(new TestMessage(current));
                    }, i);
                }
                else
                {
                    int current = i;
                    current++;
                    Publish<TestMessage>(new TestMessage(current));
                }
            }
        }

        public void Handle(StatusMessage message)
        {
            string prefix = string.Empty;
            if (message.Id == this.Id)
            {
                prefix = "*";
            }

            Console.WriteLine();

            Console.WriteLine(string.Format("{0} {1}\n\tReceived:\t{2}\n\tDuration:\t{3}\n\tRate:\t\t{4:0.00} msgs/second",
                prefix,
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

        private double CalculateRate()
        {
            double rate;
            if (endTime == DateTimeOffset.MinValue && startTime != DateTimeOffset.MinValue)
            {
                duration = DateTimeOffset.Now - startTime;
                rate = received / duration.TotalSeconds;
            }
            else
            {
                rate = received / duration.TotalSeconds;
            }

            return rate;
        }

        public void Handle(UnsubscribeCommand message)
        {
            
        }

        public void Handle(QueryConsistency message)
        {
            Console.WriteLine();
            int errors = 0;

            foreach (KeyValuePair<int, int> pair in numbers)
            {
                if (pair.Key != pair.Value)
                {
                    errors++;
                    Console.WriteLine(string.Format("{0}:{1}",
                        pair.Key,
                        pair.Value));
                }
            }
            Console.WriteLine("------------------------------------");
            Console.WriteLine("Total: " + errors.ToString());

            Console.Write("Command>");
        }

        public void Ping()
        {
            startPingTime = DateTimeOffset.Now;
            Publish<QueryPing>(new QueryPing(this.NodeId));
        }

        public void Handle(QueryPing message)
        {
            Publish<PingMessage>(new PingMessage(message.RequestedId));
        }

        public void Handle(PingMessage message)
        {
            if (message.RequestedId == this.NodeId)
            {
                string prefix = string.Empty;
                if (message.SourceId == this.NodeId)
                {
                    prefix = "* ";
                }

                DateTimeOffset endPingTime = DateTimeOffset.Now;
                TimeSpan duration = endPingTime - startPingTime;

                Console.WriteLine(string.Format("\n{0}{1}\t{2}",
                    prefix,
                    message.SourceId,
                    duration.ToString()));

                Console.Write("Command>");
            }
        }
    }
}
