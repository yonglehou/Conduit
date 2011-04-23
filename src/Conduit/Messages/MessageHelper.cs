using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit.Messages
{
    public class MessageHelper
    {
        private static Type messageNamespaceType = null;
 
        public static List<string> GetCapabilities(object obj)
        {
            List<string> capabilities = new List<string>();

            string handleName = typeof(IHandle).Name;
            Type[] interfaces = obj.GetType().GetInterfaces();

            if (messageNamespaceType == null)
            {
                messageNamespaceType = typeof(ConduitMessageAttribute);
            }

            foreach (Type i in interfaces)
            {
                if (i.Name.StartsWith(handleName))
                {
                    Type[] messageTypes = i.GetGenericArguments();
                    if (messageTypes.Count() > 0)
                    {
                        Type messageType = messageTypes[0];
                        ConduitMessageAttribute attrib = GetMessageInfo(messageType);
                        if (attrib != null)
                        {
                            if (!string.IsNullOrEmpty(attrib.Namespace))
                            {
                                capabilities.Add(attrib.Namespace);
                            }
                        }
                    }
                }
            }
            return capabilities;
        }

        public static ConduitMessageAttribute GetMessageInfo(object message)
        {
            return GetMessageInfo(message.GetType());
        }

        public static ConduitMessageAttribute GetMessageInfo(Type messageType)
        {
            string ns = string.Empty;

            if (messageNamespaceType == null)
            {
                messageNamespaceType = typeof(ConduitMessageAttribute);
            }

            object[] attributes = messageType.GetCustomAttributes(messageNamespaceType, true);
            if (attributes.Count() > 0)
            {
                ConduitMessageAttribute messageNamespace = attributes[0] as ConduitMessageAttribute;
                if (ns != null)
                {
                    return messageNamespace;
                }
            }
            return null;
        }
    }
}
