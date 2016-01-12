namespace TW.CQRS.Core.Interfaces.Domain.Event
{
    public interface IDomainEventHandler<T> : IGenericDomainEventHandler where T : IDomainEvent
    {
        void Handle(T @event);
    }
}