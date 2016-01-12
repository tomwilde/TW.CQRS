using EstateAgent.Domain.Model.Events.Property;
using TW.CQRS.Core.Domain.Aggregate;
using TW.CQRS.Core.Interfaces.Domain.Aggregate;

namespace EstateAgent.Domain.Model
{
    public class Property : AbstractAggregate, IAggregateRoot
    {
        public decimal Price { get; private set; }

        public string Name { get; private set; }

        private Property(string name, decimal price)
        {
            Name = name;
            Price = price;

            ApplyEvent(new Created() {Name = name, Price = price});
        }

        public static Property Create()
        {
            return new Property(string.Empty, 0);
        }

        protected override void RegisterEvents()
        {
            RegisterEventHandler<Created>(e =>
            {
                Name = e.Name;
                Price = e.Price;
            });
        }

        public void UpdateName(string newName)
        {
            ApplyEvent(new Updated(){NewName = newName});
        }

        public void UpdatePrice(decimal newPrice)
        {
            ApplyEvent(new Updated(){ NewPrice = newPrice});
        }
    }
}
