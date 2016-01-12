using EstateAgent.Domain.Model.Events.Property;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace EstateAgent.Domain.Model
{
    public class Property : AbstractAggregate, IAggregateRoot
    {
        public decimal Price { get; private set; }
        public string Name { get; private set; }
        public int PropertyId { get; private set; }

        private Property(int propertyId, string name, decimal price)
        {
            PropertyId = propertyId;
            Name = name;
            Price = price;

            ApplyEvent(new Created() { PropertyId = propertyId, Name = name, Price = price});
        }

        public static Property Create()
        {
            return new Property(0, string.Empty, 0);
        }

        public static Property Create(string name, decimal price)
        {
            return new Property(0, name, price);
        }

        protected override void RegisterEvents()
        {
            RegisterEventHandler<Created>(e =>
            {
                PropertyId = e.PropertyId;
                Name = e.Name;
                Price = e.Price;
            });
        }

        public void UpdateName(string newName)
        {
            ApplyEvent(new Updated()
            {
                PropertyId = PropertyId,
                NewName = newName
            });
        }

        public void UpdatePrice(decimal newPrice)
        {
            ApplyEvent(new Updated()
            {
                PropertyId = PropertyId,
                NewPrice = newPrice
            });
        }
    }
}
