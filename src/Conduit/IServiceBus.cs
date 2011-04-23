using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conduit
{
    public interface IServiceBus : IDisposable
    {
        void Open();
        void Publish<T>(T message) where T : class;
        //void Subscribe<T>(T consumer) where T : class;
        void Subscribe<T>() where T : class;
    }
}
