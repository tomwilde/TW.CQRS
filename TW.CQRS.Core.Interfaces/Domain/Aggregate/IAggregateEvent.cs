using System;

namespace TW.CQRS.Core.Interfaces.Domain.Aggregate
{
    public interface IAggregateEvent : IMessage
    {
        int AggregateId { get; }
        DateTime TimeStamp { get; }
    }
}