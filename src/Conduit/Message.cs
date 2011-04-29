using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public abstract class Message
    {
        public Message()
            : this(Guid.NewGuid(), DateTimeOffset.Now)
        {
        }

        public Message(Guid id, DateTimeOffset timeStamp)
        {
            this.Id = id;
            this.TimeStamp = timeStamp;
        }

        public Guid Id { get; private set; }
        public DateTimeOffset TimeStamp { get; private set; }
    }
}
