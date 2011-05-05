using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public class MessageHelper
    {
        public static List<string> GetCapabilities(object obj)
        {
            List<string> capabilities = new List<string>();

            string handleName = typeof(IHandle).Name;
            Type[] interfaces = obj.GetType().GetInterfaces();

            IEnumerable<Type> handles = interfaces.Where(x => x.Name.StartsWith(handleName));

            foreach (Type i in handles)
            {
                Type[] messageTypes = i.GetGenericArguments();
                if (messageTypes.Count() > 0)
                {
                    Type messageType = messageTypes[0];
                    capabilities.Add(messageType.FullName);
                }
            }
            return capabilities;
        }
    }
}
