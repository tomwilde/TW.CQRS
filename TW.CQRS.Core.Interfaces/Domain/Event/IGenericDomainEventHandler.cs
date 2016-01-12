namespace TW.CQRS.Core.Interfaces.Domain.Event
{
    public interface IGenericDomainEventHandler
    {
        string Name { get; set; }
        void Handle(IDomainEvent @event);
    }
}