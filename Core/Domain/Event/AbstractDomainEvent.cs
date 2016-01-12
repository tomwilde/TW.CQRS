using System;
using TW.CQRS.Core.Interfaces.Domain.Event;

namespace TW.CQRS.Core.Domain.Event
{
    [Serializable]
    public abstract class AbstractDomainEvent : AbstractEvent, IDomainEvent
    {
    }
}
