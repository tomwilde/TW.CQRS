using TW.CQRS.Core.Domain.Event;
using TW.CQRS.Core.Interfaces;
using TW.Commons.Interfaces;

using EstateAgent.Domain.Events;


namespace EstateAgent.Domain.EventHandlers
{
    public class PropertyRegisteredDomainEventHandler : AbstractDomainEventHandler<PropertyRegisteredDomainEvent>
    {
        private readonly INormaliser _normaliser;

        public PropertyRegisteredDomainEventHandler(ILoggerFactory loggerFactory, INormaliser normaliser) 
            : base(loggerFactory)
        {
            _normaliser = normaliser;
        }

        public override void Handle(PropertyRegisteredDomainEvent @event)
        {
            logger.InfoFormat("New Property Registered! ", @event);

            // hand off to the normaliser, this could be a message handler, external svc, wcf, whatever.

            _normaliser.Normalise(@event);
        }
    }
}