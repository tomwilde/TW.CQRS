using System;

namespace EstateAgent.Domain.Model.Events.Property
{
    [Serializable]
    public class Updated : AbstractPropertyAggregateEvent
    {
        public decimal NewPrice { get; set; }
        public string NewName { get; set; }
    }
}