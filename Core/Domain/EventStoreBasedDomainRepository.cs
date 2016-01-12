using TW.CQRS.Core.Interfaces.Domain;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Domain
{
    public class EventStoreBasedDomainRepository<T> : IDomainRepository<T> where T : IAggregateRoot, new()
    {
        private readonly IAggregateRootEventStorage _rootEventStorage;

        public EventStoreBasedDomainRepository(IAggregateRootEventStorage _rootEventStorage)
        {
            this._rootEventStorage = _rootEventStorage;
        }

        public T Load(string name)
        {
            var result = new T();

            // TODO: this is very inefficient,  load from last snapshot and replay events up to latest

            result.LoadFromHistory(_rootEventStorage.GetEvents(name));
            return result;
        }

        public void Save(T obj)
        {
            // TODO: snapshots

            _rootEventStorage.SaveEvents(obj);
        }
    }
}
