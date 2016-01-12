using System.Threading.Tasks;
using TW.CQRS.Core.Interfaces.Domain.Event;
using TW.Commons.Interfaces;

namespace TW.CQRS.Core.Domain.Event
{
    public class DomainEventBus : AbstractBus<IGenericDomainEventHandler>, IDomainEventBus
    {
        public DomainEventBus(DomainEventHandlerFactory handlerFactory, ILoggerFactory loggerFactory) 
            : base(loggerFactory, handlerFactory)
        {
        }

        public void Publish<T>(T message) where T : IDomainEvent
        {
            logger.Info(string.Format("Publishing event of type:{0} content:{1}", message.GetType(), message.ToString()));

            if (Handlers.ContainsKey(typeof(T)))
            {
                Task.Run(() => Handlers[typeof(T)].Handle(message));
            }
            else
            {
                logger.Warn(string.Format("DomainEvent not handled: {0}", typeof(T).Name));
            }
        }
    }
}