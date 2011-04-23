using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public abstract class AnnounceIdentity : Message
    {
        public AnnounceIdentity(string name, string @namespace, IList<string> capabilities)
        {
            this.Name = name;
            this.Namespace = @namespace;
            this.Capabilities = capabilities;
        }
        
        /// <summary>
        /// This is the friendly name of your Component or Service.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Namespace identifier for the Component or Service within the distributed system. Using a Uri is suggested.
        /// </summary>
        /// <example>
        /// http://company.com/service/myservice
        /// or
        /// http://company.com/component/mycomponent
        /// </example>
        public string Namespace { get; private set; }

        /// <summary>
        /// List of capabilities this Component or Service supports.
        /// </summary>
        /// <example>
        /// http://company.com/protocol/capabilityname
        /// or
        /// http://company.com/protocol/capabilityname#version
        /// or
        /// service:protocol:capability
        /// </example>
        public IList<string> Capabilities { get; private set; }
    }
}
