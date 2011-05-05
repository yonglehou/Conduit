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
                    case "check":
                        conduit.Publish<QueryConsistency>();
                        break;
                    case "clear":
                        conduit.Publish<ClearCommand>();
                        break;
                    case "status":
                        conduit.Publish<QueryStatus>();
                        break;
                    case "test":
                        int cnt = int.Parse(cmdline[1]);
                        bool async = false;
                        if (cmdline.Length > 2 && cmdline[2].ToLowerInvariant() == "async")
                        {
                            async = true;
                        }
                        conduit.Publish<StartCommand>(new StartCommand(cnt, async));
                        break;
                    case "ping":
                        LoadComponent component = conduit.Components[0] as LoadComponent;
                        component.Ping();
                        break;
                    case "unsubscribe":
                        conduit.Publish<UnsubscribeCommand>();
                        break;
                }
            }
        }
    }
}
