using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample.Messages.Commands
{
    [ConduitMessage("http://Conduit/Samples/MessageLoadSample/ClearCommand")]
    public class ClearCommand : Message
    {
        public ClearCommand()
        {

        }
    }
}
