using System;
using System.Collections.Generic;
using System.Threading;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Domain.Aggregate
{
    public abstract class AbstractAggregate : IAggregate
    {
        public Guid Id { get; set; }
        
        public string EntityName { get; private set; }

        protected AbstractAggregate()
        {
            Id = Guid.NewGuid();

            EntityName = GetType().Name;

            RegisterEvents();
        }

        private readonly Dictionary<Type, Action<IAggregateEvent>> eventHandlers = new Dictionary<Type, Action<IAggregateEvent>>();
        private readonly LinkedList<IAggregateEvent> appliedEvents = new LinkedList<IAggregateEvent>();

        // used to ensure the applied events list is consistent
        private readonly ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

        protected abstract void RegisterEvents();
        
        protected void CheckIsCreated(IAggregateEvent @event)
        {
            if (Id == new Guid() && (!(@event is IAggregateCreationEvent)))
            {
                throw new Exception("The object has not been 'created' yet!");
            }
        }

        protected void RegisterEventHandler<T>(Action<T> eventHandler) where T : class, IAggregateCreationEvent
        {
            eventHandlers.Add(typeof(T), @event => eventHandler(@event as T));
        }

        public void LoadFromHistory(LinkedList<IAggregateEvent> history)
        {   
            if (history == null) return;
            if (history.First == null && history.Last == null) return;

            foreach (var @event in history)
            {
                InvokeEventHandlers(@event);
            }
        }

        private void InvokeEventHandlers(IAggregateEvent @event)
        {
            var eventType = @event.GetType();

            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Invoke(@event);
            }
        }

        public void ApplyEvent(IAggregateEvent @event)
        {
            CheckIsCreated(@event);

            InvokeEventHandlers(@event);

            readerWriterLockSlim.EnterWriteLock();
            appliedEvents.AddLast(@event);
            readerWriterLockSlim.ExitWriteLock();
        }

        public LinkedList<IAggregateEvent> GetAllChanges()
        {
            readerWriterLockSlim.EnterReadLock();
            var events = new LinkedList<IAggregateEvent>(appliedEvents);
            readerWriterLockSlim.ExitReadLock();
            return events;
        }

        public void MarkCommitted()
        {
            readerWriterLockSlim.EnterWriteLock();
            appliedEvents.Clear();
            readerWriterLockSlim.ExitWriteLock();
        }
    }
}