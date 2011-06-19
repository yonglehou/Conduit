using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Munq;

namespace Conduit
{
    internal sealed class ServiceLocator
    {
        private static volatile ServiceLocator instance;
        private static object syncRoot = new Object();

        private IocContainer container = null;
        private readonly IDependencyResolver resolver;

        public static ServiceLocator Current
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ServiceLocator();
                    }
                }

                return instance;
            }
        }

        public IRegistration RegisterInstance<TType>(TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(instance);
        }

        public IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class
        {
            return container.Register<TType>(func);
        }

        public IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class
        {
            return container.RegisterInstance<TType>(name, instance);
        }

        public bool CanResolve<TType>() where TType : class
        {
            return container.CanResolve<TType>();
        }

        public bool CanResolve<TType>(string name) where TType : class
        {
            return container.CanResolve<TType>(name);
        }

        public TType Resolve<TType>() where TType : class
        {
            return container.Resolve<TType>();
        }

        public TType Resolve<TType>(string name) where TType : class
        {
            return container.Resolve<TType>(name);
        }

        public IEnumerable<TType> ResolveAll<TType>() where TType : class
        {
            return container.ResolveAll<TType>();
        }

        public void Remove<TType>() where TType : class
        {
            IEnumerable<IRegistration> registrations = container.GetRegistrations<TType>();
            foreach (IRegistration registration in registrations)
            {
                container.Remove(registration);
            }
        }

        public void Remove<TType>(string name) where TType : class
        {
            IEnumerable<IRegistration> registrations = container.GetRegistrations<TType>();
            foreach (IRegistration registration in registrations)
            {
                if (registration.Name == name)
                {
                    container.Remove(registration);
                }
            }
        }

        public ServiceLocator()
        {
            container = new IocContainer();
            resolver = container;
        }
    }
}
