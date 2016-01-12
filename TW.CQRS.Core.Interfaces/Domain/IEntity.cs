using System.Collections.Generic;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Interfaces.Domain
{
    public interface IEntity
    {
        string EntityName { get; }

        void LoadFromHistory(LinkedList<IAggregateEvent> history);
        void ApplyEvent(IAggregateEvent @event);
        LinkedList<IAggregateEvent> GetAllChanges();
        void MarkCommitted();
    }
}