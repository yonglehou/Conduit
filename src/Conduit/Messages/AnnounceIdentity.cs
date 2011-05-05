using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public abstract class AnnounceIdentity : Message
    {
        public AnnounceIdentity(string name, string type, IList<string> capabilities)
        {
            this.Name = name;
            this.Type = type;
            this.Capabilities = capabilities;
        }
        
        /// <summary>
        /// This is the friendly name of your Component or Service.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Type identifier for the Component or Service within the distributed system.
        /// </summary>
        /// <example>
        /// Conduit.Messages.FindAvailableServices
        /// </example>
        public string Type { get; private set; }

        /// <summary>
        /// List of capabilities this Component or Service supports.
        /// </summary>
        /// <example>
        /// Conduit.Messages.FindAvailableServices
        /// </example>
        public IList<string> Capabilities { get; private set; }
    }
}
