using System;
using System.Collections.Generic;

namespace TW.CQRS.Core.Interfaces.Domain.Aggregate
{
    public interface IAggregateRootEventStorage
    {
        LinkedList<IAggregateEvent> GetEvents(string aggregateName);

        void SaveEvents(IAggregateRoot aggregateRoot);
    }
}