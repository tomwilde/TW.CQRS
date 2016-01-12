using System;
using TW.CQRS.Core.Interfaces.Domain;
using TW.CQRS.Core.Interfaces.Domain.Event;

namespace TW.CQRS.Core.Domain.Event
{
    public class DomainEventBusException<T> : AbstractBusException where T : IDomainEvent
    {
        public DomainEventBusException(string error) : base(error)
        {
        }

        public DomainEventBusException(Exception innerException) : base(string.Format("Domain Command Bus Exceptions for message type: {0}", typeof (T)), innerException)
        {
        }
    }
}