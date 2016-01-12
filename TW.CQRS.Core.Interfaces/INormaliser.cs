using TW.CQRS.Core.Interfaces.Domain.Event;

namespace TW.CQRS.Core.Interfaces
{
    public interface INormaliser
    {
        void Normalise(IDomainEvent @event);
    }
}