﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public class MessageNamespaceMissingException : Exception
    {
        public MessageNamespaceMissingException(string message)
            : base(message)
        {
        }
    }
}
