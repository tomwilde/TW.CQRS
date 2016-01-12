using System;
using TW.CQRS.Core.Domain.Aggregate;

namespace EstateAgent.Domain.Model.Events.Property
{
    [Serializable]
    public class Created : AbstractAggregateCreationEvent<Model.Property>
    {
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}
