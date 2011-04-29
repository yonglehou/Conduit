using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample
{
    [Conduit("http://Conduit/Samples/MessageLoadSample")]
    public class LoadConduit : Conduit.Conduit
    {
        public LoadConduit(IServiceBus bus) 
            : base(bus)
        {
            this.Components.Add(new LoadComponent());
        }
    }
}
