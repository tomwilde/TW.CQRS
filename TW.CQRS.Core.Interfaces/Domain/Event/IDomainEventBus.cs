namespace TW.CQRS.Core.Interfaces.Domain.Event
{
    public interface IDomainEventBus : IBus
    {
        void Publish<T>(T message) where T : IDomainEvent;
    }
}