using System;
using TW.CQRS.Core.Domain.Aggregate;

namespace EstateAgent.Domain.Model.Events.Property
{
    [Serializable]
    public class Updated : AbstractAggregateEvent<Model.Property>
    {
        public decimal? NewPrice { get; set; }
        public string NewName { get; set; }
    }
}