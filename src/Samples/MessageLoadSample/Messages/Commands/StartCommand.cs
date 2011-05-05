using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages.Commands
{
    public class StartCommand : Message
    {
        public StartCommand()
        {

        }

        public StartCommand(int count, bool async)
        {
            this.Count = count;
            this.Async = async;
        }

        public int Count { get; set; }
        public bool Async { get; set; }
    }
}
