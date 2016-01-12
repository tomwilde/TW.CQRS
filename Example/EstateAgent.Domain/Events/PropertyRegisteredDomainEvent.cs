using TW.CQRS.Core.Domain.Event;

namespace EstateAgent.Domain.Events
{
    public class PropertyRegisteredDomainEvent : AbstractDomainEvent
    {
        private readonly int propertyId;
        private readonly string name;
        private readonly decimal price;

        public PropertyRegisteredDomainEvent(int propertyId, string name, decimal price)
        {
            this.propertyId = propertyId;
            this.name = name;
            this.price = price;
        }

        public string Name
        {
            get { return name; }
        }

        public decimal Price
        {
            get { return price; }
        }

        public int PropertyId 
        { 
            get
            {
                return propertyId;
            }
        }
    }
}