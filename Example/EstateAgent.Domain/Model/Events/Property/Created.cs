using System;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace EstateAgent.Domain.Model.Events.Property
{
    [Serializable]
    public class Created : AbstractPropertyAggregateEvent, IAggregateCreationEvent
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}
