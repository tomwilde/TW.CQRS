using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using TW.Commons.Interfaces;

namespace TW.Commons.IoC
{
    /// <summary>
    /// Wrapper for the Unity container - use in factories to create concrete instances
    /// </summary>
    public class FactoryTypeContainer : IFactoryTypeContainer
    {
        protected static IUnityContainer container;
        protected static bool isInitialised = false;

        protected static readonly List<Type> allowedTypes = new List<Type>();

        public static void Initialise(IUnityContainer theContainer)
        {
            if (theContainer == null) throw new ArgumentNullException(); 

            container = theContainer;
            isInitialised = true;
        }

        public object Resolve(Type type)
        {
            TypeResolutionPreChecks(type);
            return container.Resolve(type);
        }

        public T Resolve<T>()
        {
            TypeResolutionPreChecks(typeof(T));
            return container.Resolve<T>();
        }

        public List<T> ResolveAll<T>()
        {
            TypeResolutionPreChecks(typeof(T));
            return container.ResolveAll<T>().ToList();
        }

        protected virtual void TypeResolutionPreChecks(Type type)
        {
            if (!isInitialised)
                throw new InvalidOperationException("Container has not been initialized.");

            if (!allowedTypes.Contains(type))
                throw new InvalidOperationException(string.Format("Type resolution for type {0} is not allowed.", type));
        }

        public void RegisterType(Type type)
        {
            if (!allowedTypes.Contains(type))
            {
                allowedTypes.Add(type);
                container.RegisterType(type);
            }
        }

        public void Configure(Type type)
        {
            if (!isInitialised)
                throw new InvalidOperationException("Container has not been initialized.");

            var list = type.GetCustomAttributes(typeof (FactoryType), false).ToList();

            list.ConvertAll(o => (o as FactoryType).TypeToRegisterWithContainer).ForEach(typeToReg =>
            {
                if (!allowedTypes.Contains(typeToReg)) allowedTypes.Add(typeToReg);
            });
            
            list.ForEach(t => container.RegisterType((t as FactoryType).TypeToRegisterWithContainer));
        }
    }
}