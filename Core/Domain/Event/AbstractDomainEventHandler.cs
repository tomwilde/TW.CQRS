using System;
using System.Transactions;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Event
{
    /// <summary>
    /// Sagas and components external to the Domain will handle Domain Events
    /// </summary>
    public abstract class AbstractDomainEventHandler<T> : AbstractDomainHandler<T>, IDomainEventHandler<T> where T : class, IDomainEvent
    {
        protected AbstractDomainEventHandler(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        public void Handle(IDomainEvent @event)
        {
            if (@event is T)
            {
                Handle(@event as T);
            }
        }

        public abstract void Handle(T @event);

        protected void WithinTransaction(T command, Action<T> action) 
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    action(command);

                    transaction.Complete();
                }
            }
            catch (Exception ex)
            {
                RaiseException(ex, command);
            }
        }
    }
}