using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit.Bus.MassTransit;
using MessageLoadSample.Messages;
using MessageLoadSample.Messages.Commands;
using MessageLoadSample.Messages.Queries;
using System.Threading;

namespace MessageLoadSample
{
    class Program
    {
        private static LoadConduit conduit = null;

        static void Main(string[] args)
        {
            MassTransitBus bus = new MassTransitBus();
            conduit = new LoadConduit(bus);
            conduit.Open();

            string cmd = string.Empty;
            while (cmd.ToLowerInvariant() != "exit")
            {
                Console.Write("Command>");
                string[] cmdline = Console.ReadLine().Split(' ');
                cmd = cmdline[0];

                switch (cmd)
                {
                    case "clear":
                        conduit.Bus.Publish<ClearCommand>();
                        break;
                    case "status":
                        conduit.Bus.Publish<QueryStatus>();
                        break;
                    case "test":
                        Test(int.Parse(cmdline[1]));
                        break;
                }
            }
        }

        private static void Test(int count)
        {
            //DateTimeOffset startTime = DateTimeOffset.Now;

            conduit.Bus.Publish<StartCommand>(new StartCommand(count));
            //for(int i=0; i<count; i++)
            //{
            //    ThreadPool.QueueUserWorkItem((o) =>
            //    {
            //        conduit.Bus.Publish<TestMessage>(new TestMessage(i));
            //    });
                
            //}

            //DateTimeOffset endTime = DateTimeOffset.Now;
            //TimeSpan duration = endTime - startTime;
            //Console.WriteLine(string.Format("{0} messages sent in {1} for a rate of {2:0.00} msgs/second",
            //    count.ToString(),
            //    duration.ToString(),
            //    count / duration.TotalSeconds));
        }
    }
}
