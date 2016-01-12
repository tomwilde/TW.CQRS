using TW.CQRS.Core.Interfaces.Domain;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Event
{
    public class DomainEventHandlerFactory : AbstractHandlerFactory<IGenericDomainEventHandler>
    {
        public DomainEventHandlerFactory(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            handlerTypeName = "Domain Event";
        }
    }
}