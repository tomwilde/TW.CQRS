using System;

namespace TW.CQRS.Core.Interfaces.Domain
{
    public interface IDomainRepository<T>
    {
        T Load(string name);
        void Save(T obj);
    }
}