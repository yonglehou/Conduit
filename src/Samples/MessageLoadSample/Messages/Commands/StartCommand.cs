using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages.Commands
{
    [ConduitMessage("http://Conduit/Samples/MessageLoadTest/ClearCommand")]
    public class StartCommand : Message
    {
        public StartCommand()
        {

        }

        public StartCommand(int count)
        {
            this.Count = count;
        }

        public int Count { get; set; }
    }
}
