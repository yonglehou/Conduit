using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;
using Conduit.Bus.MassTransit;
using MessageLoadSample.Messages;
using MessageLoadSample.Messages.Commands;
using MessageLoadSample.Messages.Queries;
using System.Threading;

namespace MessageLoadSample
{
    class Program
    {
        static void Main(string[] args)
        {
            MassTransitBus bus = new MassTransitBus();
            ConduitNode.Create()
                .WithServiceBus(bus)
                .Open();

            CommandActor actor = new CommandActor();
            
            string cmd = string.Empty;
            while (cmd.ToLowerInvariant() != "exit")
            {
                Console.Write("Command>");
                string[] cmdline = Console.ReadLine().Split(' ');
                cmd = cmdline[0];

                switch (cmd)
                {
                    case "check":
                        actor.Publish<QueryConsistency>();
                        break;
                    case "clear":
                        actor.Publish<ClearCommand>();
                        break;
                    case "status":
                        actor.Publish<QueryStatus>();
                        break;
                    case "test":
                        int cnt = int.Parse(cmdline[1]);
                        bool async = false;
                        if (cmdline.Length > 2 && cmdline[2].ToLowerInvariant() == "async")
                        {
                            async = true;
                        }
                        actor.Publish<StartCommand>(new StartCommand(cnt, async));
                        break;
                    case "ping":
                        //CommandActor component = conduit.Components[0] as CommandActor;
                        //component.Ping();
                        break;
                    case "unsubscribe":
                        actor.Publish<UnsubscribeCommand>();
                        break;
                }
            }
        }
    }
}
