using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Conduit;

namespace MessageLoadSample
{
    public class LoadConduit : ConduitNode
    {
        public LoadConduit(IServiceBus bus) 
            : base(bus)
        {
            this.Components.Add(new LoadComponent());
        }
    }
}
