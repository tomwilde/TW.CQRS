using System;
using System.Collections.Generic;

namespace TW.Commons.Interfaces
{
    /// <summary>
    /// Wrapper for an IoC container - use in factories to create concrete instances
    /// </summary>
    public interface IFactoryTypeContainer
    {
        object Resolve(Type type);
        T Resolve<T>();
        List<T> ResolveAll<T>();
        void Configure(Type type);
        void RegisterType(Type type);
    }
}