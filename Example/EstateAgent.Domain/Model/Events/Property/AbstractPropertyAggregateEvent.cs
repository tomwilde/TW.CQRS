using TW.CQRS.Core.Domain.Aggregate;

namespace EstateAgent.Domain.Model.Events.Property
{
    public class AbstractPropertyAggregateEvent : AbstractAggregateEvent<Model.Property>
    {
        public int PropertyId { get; set; }
    }
}