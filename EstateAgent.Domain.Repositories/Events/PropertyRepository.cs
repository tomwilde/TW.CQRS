using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EstateAgent.Domain.Model;
using TW.CQRS.Core.Interfaces.Domain;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace EstateAgent.Domain.Repositories.Events
{
    public abstract class EFEventRepository<C,S> : IAggregateEventStorage
        where C : DbContext, new() 
        where S : IAggregateEvent 
    {
        protected readonly C context;

        protected Func<List<S>> getAll;
        protected Action<S> addEvent;

        protected EFEventRepository()
        {
            context = new C();
        }

        public LinkedList<IAggregateEvent> GetEvents()
        {
            return new LinkedList<IAggregateEvent>(getAll().Cast<IAggregateEvent>());
        }

        public void Save(IEntity aggregateRoot)
        {
            aggregateRoot.GetAllChanges().Cast<S>().ToList().ForEach(pe => addEvent(pe));
            context.SaveChanges();
        }
    }

    public class PropertyEvent : IAggregateEvent
    {
        public Guid Id { get; private set; }
        public DateTime TimeStamp { get; private set; }
    }

    public class EventsContext : DbContext
    {        
        public DbSet<PropertyEvent> Events { get; set; }
    }

    public class PropertyEventRepository : EFEventRepository<EventsContext, PropertyEvent> 
    {
        public PropertyEventRepository() 
        {
            getAll = () => context.Events.ToList();
            addEvent = pe => context.Events.Add(pe);
        }
    }
}
