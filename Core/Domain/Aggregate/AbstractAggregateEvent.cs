using System;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace TW.CQRS.Core.Domain.Aggregate
{
    [Serializable]
    public abstract class AbstractAggregateEvent<T> : AbstractEvent, IAggregateEvent
    {
    }
}